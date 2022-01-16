using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Google.Apis.Auth.OAuth2.Flows;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Newtonsoft.Json;
using System.Threading;
using Google.Apis.Auth.OAuth2.Responses;
using StackExchange.Redis;
using Google.Apis.Calendar.v3.Data;
using System.Text;
using Appts.Models.Document;
using Appts.Dal.RedisCache;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Models.Rest;

namespace Appts.Web.Ui.Scheduler.Services
{
  /// <summary>
  /// Bridges gap between Google API and web app.
  /// 
  /// Note, the client will be initialized to perform actions
  /// on behalf of a Service Provider, and not necessarily
  /// the currently logged in user.
  /// </summary>
  public class GoogleCalendarClient : IGoogleCalendarClient
  {
    private readonly HttpClient _client;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly IApiClient _apiClient;
    private readonly IRedisConnectionFactory _cache;
    private string _serviceProviderUserId;
    private string _host;
    private readonly IHttpContextResolverService _httpContext;
    private readonly ITokenCacheRepository _tokenCacheRepository;
    private IDatabase _db;

    /// <summary>
    /// Will only be true once initialized with Service Provider's user id.
    /// </summary>
    private bool _initialized;

    private string _accessToken;

    public GoogleCalendarClient(
      HttpClient client, IConfiguration config,
      IApiClient apiClient, IRedisConnectionFactory cache,
      IHttpContextResolverService httpContext, ITokenCacheRepository tokenCacheRepository)
    {
      this._client = client;
      _clientId = config["GoogleCalendarApi:client_id"];
      _clientSecret = config["GoogleCalendarApi:clientsecret"];
      _apiClient = apiClient;
      _cache = cache;
      _tokenCacheRepository = tokenCacheRepository;
      _httpContext = httpContext;
      _host = httpContext.GetHost();
    }

    /// <summary>
    /// Initialize client on behalf of the service provider.
    /// </summary>
    /// <param name="serviceProviderUserId">The user id of the target SP</param>
    public void Initialize(string serviceProviderUserId)
    {
      _serviceProviderUserId = serviceProviderUserId;
      _initialized = true;
    }

    #region "Caching"

    private void InitializeCache()
    {
      try
      {
        _db = _cache.Connection().GetDatabase();
      }
      catch (RedisException ex)
      {
      }
    }
    private string TryGetCachedAccessToken()
    {
      string tokenKey = CacheHelper.GetGoogleCalendarTokenKey(_serviceProviderUserId, "access");
      string cachedToken = "";

      if (_db != null)
      {
        try
        {
          cachedToken = _db.StringGet(tokenKey);
        }
        catch (RedisException ex)
        {
        }
      }

      return cachedToken;
    }
    private void TryRemoveCachedAccessToken()
    {
      string tokenKey = CacheHelper.GetGoogleCalendarTokenKey(_serviceProviderUserId, "access");
      if (_db != null)
      {
        try
        {
          _db.KeyDelete(tokenKey);
        }
        catch (RedisException ex)
        {

        }
      }
    }
    async Task TryCacheTokensAsync(TokenResponse tokens)
    {
      try
      {
        if (_db == null)
        {
          return;
        }
        string accessTokenKey = CacheHelper.GetGoogleCalendarTokenKey(_serviceProviderUserId, "access");
        string refreshTokenKey = CacheHelper.GetGoogleCalendarTokenKey(_serviceProviderUserId, "refres");

        // access tokens good for 60 mins, cached for 59
        TimeSpan accessTokenExpiration = TimeSpan.FromSeconds((double)(tokens.ExpiresInSeconds - 60));

        // 3 day life for refresh token
        TimeSpan refreshTokenExpiration = DateTime.UtcNow.AddDays(3).Subtract(DateTime.UtcNow);

        _db.StringSet(accessTokenKey, tokens.AccessToken, accessTokenExpiration);
        _db.StringSet(refreshTokenKey, tokens.RefreshToken, refreshTokenExpiration);
      }
      catch (RedisException ex)
      {
      }
    }

    #endregion

    #region "Auth"

    /// <summary>
    /// Trys to retreive the bearer token for a given service provider.
    /// 
    /// Precondition: the client has been intialized with the user id
    /// of the service provider
    /// </summary>
    /// <returns></returns>
    public async Task TryGetBearerToken()
    {
      if (!_initialized)
        throw new InvalidOperationException("Must initialize client with Service Provider's UserId before using.");

      InitializeCache();
      string cachedAccessToken = TryGetCachedAccessToken();

      if (!string.IsNullOrEmpty(cachedAccessToken))
      {
        _accessToken = cachedAccessToken;
      }
      else
      {
        var tokenCache = _tokenCacheRepository.GetAsync(
          _serviceProviderUserId, CalendarService.Scope.Calendar)
          .GetAwaiter().GetResult();
        if (tokenCache == null || tokenCache.RefreshToken == null)
        {
          // need to go thru auth code flow
          // wont do that here. this code expects that that flow has been completed
          // in previous step of application.
          // therefor, not finding refresh token would be considered exceptional
          throw new TokenResponseException(new TokenErrorResponse()
          {
            Error = "refresh_token_not_found",
            ErrorDescription = "No access or refresh tokens were found; must re-attempt authorization code flow"
          });
        }
        string refreshToken = tokenCache.RefreshToken;
        TokenResponse newTokens = RenewToken(refreshToken).GetAwaiter().GetResult();
        _accessToken = newTokens.AccessToken;
        // recache
        await _tokenCacheRepository.UpdateAsync(
          _serviceProviderUserId, CalendarService.Scope.Calendar, new TokenCache()
          {
            AccessToken = _accessToken
          });
        TryCacheTokensAsync(newTokens);
      }
    }
    private AuthorizationCodeFlow GetGoogleAuthorizationCodeFlow(params string[] scopes)
    {
      var initializer = new GoogleAuthorizationCodeFlow.Initializer()
      {
        ClientSecrets = new ClientSecrets()
        {
          ClientId = _clientId,
          ClientSecret = _clientSecret
        },
        Scopes = scopes
      };

      return new GoogleAuthorizationCodeFlow(initializer);
    }

    public string GetAuthorizationUrl()
    {
      var redirectUrl = $"https://{_host}/GoogleOAuth/AuthorizationCode";
      var scopes = new[] { CalendarService.Scope.Calendar };
      var googleAuthorizationCodeFlow = this.GetGoogleAuthorizationCodeFlow(scopes);
      var codeRequestUrl = googleAuthorizationCodeFlow.CreateAuthorizationCodeRequest(redirectUrl);
      codeRequestUrl.ResponseType = "code";
      // Build the url
      var authorizationUrl = codeRequestUrl.Build();

      // Give it back to our caller for the redirect
      return authorizationUrl.ToString();
    }
    public async Task RevokeCalendarApiAccessForUserAsync()
    {
      var redirectUrl = $"https://{_host}/GoogleOAuth/AuthorizationCode";
      var scopes = new[] { CalendarService.Scope.Calendar };
      var googleAuthorizationCodeFlow = this.GetGoogleAuthorizationCodeFlow(scopes);
      await TryGetBearerToken();
      googleAuthorizationCodeFlow.RevokeTokenAsync(
        _serviceProviderUserId, _accessToken, CancellationToken.None).GetAwaiter().GetResult();
      TryRemoveCachedAccessToken();
    }

    /// <summary>
    /// This endpoint is hit after user authorizes and gives consent.
    /// 
    /// We retreive the authorization code and exchange it for 
    /// access and refresh tokens.
    /// 
    /// Side effects:
    ///   - Access token is cached in Redis
    ///   - Access and refresh token is saved in Cosmos
    /// </summary>
    /// <param name="code">Authorization code from google for user</param>
    /// <returns>Tokens from google for user</returns>
    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
      var redirectUrl = $"https://{_host}/GoogleOAuth/AuthorizationCode";

      var scopes = new[] { CalendarService.Scope.Calendar };
      var googleAuthorizationCodeFlow = this.GetGoogleAuthorizationCodeFlow(scopes);
      var token = await googleAuthorizationCodeFlow.ExchangeCodeForTokenAsync(
        _serviceProviderUserId, code, redirectUrl, CancellationToken.None);

      await SaveCalendarAndTokens(token);

      TryCacheTokensAsync(token);

      return token;
    }

    public async Task<TokenResponse> RenewToken(string refreshToken)
    {
      var redirectUrl = $"https://{_host}/GoogleOAuth/AuthorizationCode";

      var scopes = new[] { CalendarService.Scope.Calendar };
      var googleAuthorizationCodeFlow = this.GetGoogleAuthorizationCodeFlow(scopes);
      var newToken = await googleAuthorizationCodeFlow.RefreshTokenAsync(
        _serviceProviderUserId, refreshToken, CancellationToken.None);

      //TODO: update cache as required

      return newToken;
    }


    //todo: save scope!! currently scope is null
    private async Task SaveCalendarAndTokens(TokenResponse tokens)
    {
      var tokenCache = new TokenCache()
      {
        AccessToken = tokens.AccessToken,
        ExpiresInSeconds = (long)tokens.ExpiresInSeconds,
        RefreshToken = tokens.RefreshToken,
        Id = Guid.NewGuid().ToString(),
        UserId = _serviceProviderUserId,
        Scopes = new string[]
        {
          CalendarService.Scope.Calendar
        }
      };
      var calendar = new Appts.Models.Document.Calendar()
      {
        Provider = "Google",
        Id = Guid.NewGuid().ToString(),
        AddedOn = DateTime.Now,
        UserId = _serviceProviderUserId,
        Active = true
      };

      var request = new AddConnectedCalendarRequest()
      {
        TokenCache = tokenCache,
        CalendarToAdd = calendar,
        UserId = _serviceProviderUserId
      };

      await _apiClient.PostNoReturnAsync<AddConnectedCalendarRequest>(
      request, "/api/calendars/AddConnectedCalendar");
    }


    #endregion

    public async Task CancelCalendarEvent(string eventId, DateTime start, DateTime end)
    {
      await TryGetBearerToken();
      var svc = new CalendarService();

      Event evnt = new Event()
      {
        Status = "cancelled",
        Start = new EventDateTime()
        {
          DateTime = start
        },
        End = new EventDateTime()
        {
          DateTime = end
        }
      };

      EventsResource.UpdateRequest request = svc.Events.Update(evnt, "primary", eventId);
      request.OauthToken = _accessToken;

      var result = request.Execute();
    }

    public async Task<string> CreateCalendarEvent(Appointment appointment, string spVanityUrl)
    {
      await TryGetBearerToken();

      var evnt = new Event()
      {
        Summary = appointment.AppointmentTypeBreif,
        //Description = appointment.Notes,
        Description = GetReminderDescriptionHtmlBody(appointment, spVanityUrl),

        //Description = $"{_host}/Appointment/Cancel/{appointment.Id}",

        Start = new EventDateTime()
        {
          //DateTime = appointment.StartTime,
          DateTimeRaw = appointment.StartTime.ToString("o"),
          TimeZone = appointment.TimeZoneId
        },
        End = new EventDateTime()
        {
          //DateTime = appointment.EndTime,
          DateTimeRaw = appointment.EndTime.ToString("o"),
          TimeZone = appointment.TimeZoneId
        },

        Attendees = new List<EventAttendee>()
        {
          new EventAttendee()
          {
            Email = appointment.ClientEmail,
            ResponseStatus = "accepted"
          }
        },

        Organizer = new Event.OrganizerData()
        {
          Email = "derosea7@gmail.com"
        }
      };



      // can get this from business user settings?
      evnt.Reminders = new Event.RemindersData()
      {
        Overrides = new List<EventReminder>()
        {
          new EventReminder()
          {
            Minutes = 60,
            Method = "email"
          }
        },
        UseDefault = false
      };
      var svc = new CalendarService();


      EventsResource.InsertRequest request = svc.Events.Insert(evnt, "primary");
      request.OauthToken = _accessToken;
      request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;
      var result = request.Execute();

      // generated id of event
      return result.Id;
    }

    // for dev
    public async Task CreateCalendarEvent()
    {
      await TryGetBearerToken();

      var evnt = new Event()
      {
        Summary = "created from integration test",
        Start = new EventDateTime()
        {
          DateTime = DateTime.Now.AddHours(1),
          TimeZone = "America/Phoenix"
        },
        End = new EventDateTime()
        {
          DateTime = DateTime.Now.AddHours(2),
          TimeZone = "America/Phoenix"
        }
      };

      //evnt.Description = GetEmailMarkup();
      evnt.Reminders = new Event.RemindersData()
      {
        Overrides = new List<EventReminder>()
        {
          new EventReminder()
          {
            Minutes = 55,
            Method = "email"
          }
        },
        UseDefault = false
      };
      var svc = new CalendarService();


      EventsResource.InsertRequest request = svc.Events.Insert(evnt, "primary");
      request.OauthToken = _accessToken;

      var result = request.Execute();
    }

    public async Task<string> GetEventsJsonAsync()
    {
      await TryGetBearerToken();
      var svc = new CalendarService();

      EventsResource.ListRequest request = svc.Events.List("primary");
      //request.OauthToken = "accessToken";
      request.OauthToken = _accessToken;
      request.TimeMin = DateTime.Now;
      request.ShowDeleted = false;
      request.SingleEvents = true;
      request.MaxResults = 10;
      request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

      Events events = request.Execute();
      List<string> eList = new List<string>();
      if (events.Items != null && events.Items.Count > 0)
      {
        foreach (var eItem in events.Items)
        {
          string when = eItem.Start.DateTime.ToString();
          if (String.IsNullOrEmpty(when))
          {
            when = eItem.Start.Date;
          }

          eList.Add($"{eItem.Summary} ({when})");
        }
      }
      else
      {
        eList.Add("no upcoming events");
      }

      return JsonConvert.SerializeObject(eList, Formatting.Indented);
    }

    #region "Email Markup stuff"

    private string GetReminderDescriptionHtmlBody(Appointment appt, string spVanityUrl)
    {
      return "<html>" +
        "<body>" +
        "<p>" + appt.Notes + "</p>" +
        $"<p><a href=\"https://{_host}/Appointment/Cancel/{spVanityUrl}?apptId={appt.Id}\">Cancel</a></p>" +
        "</body>" +
        "</html>";
    }

    private string GetEmailMarkup()
    {
      StringBuilder markup = new StringBuilder();

      markup.AppendLine(" <html> ");
      markup.AppendLine("   <body> ");
      markup.AppendLine("     <script type=\"application/ld+json\"> ");
      markup.AppendLine("     { ");
      markup.AppendLine("       \"@context\":              \"http://schema.org\", ");
      markup.AppendLine("       \"@type\":                 \"EventReservation\", ");
      markup.AppendLine("       \"reservationNumber\":     \"IO12345\", ");
      markup.AppendLine("       \"underName\": { ");
      markup.AppendLine("         \"@type\":               \"Person\", ");
      markup.AppendLine("         \"name\":                \"Adam DeRose\" ");
      markup.AppendLine("       }, ");
      markup.AppendLine("       \"reservationFor\": { ");
      markup.AppendLine("         \"@type\":               \"Event\", ");
      markup.AppendLine("         \"name\":                \"Speech Therapy\", ");
      markup.AppendLine("         \"startDate\":           \"2019-09-27T10:30:00-07:00\", ");
      markup.AppendLine("         \"location\": { ");
      markup.AppendLine("           \"@type\":             \"Place\", ");
      markup.AppendLine("           \"name\":              \"Some house\", ");
      markup.AppendLine("           \"address\": { ");
      markup.AppendLine("             \"@type\":           \"PostalAddress\", ");
      markup.AppendLine("             \"streetAddress\":   \"800 Howard St.\", ");
      markup.AppendLine("             \"addressLocality\": \"San Francisco\", ");
      markup.AppendLine("             \"addressRegion\":   \"CA\", ");
      markup.AppendLine("             \"postalCode\":      \"94103\", ");
      markup.AppendLine("             \"addressCountry\":  \"US\" ");
      markup.AppendLine("           } ");
      markup.AppendLine("         } ");
      markup.AppendLine("       } ");
      markup.AppendLine("     } ");
      markup.AppendLine("     </script> ");
      markup.AppendLine("     <p> ");
      markup.AppendLine("       Dear John, thanks for booking your Google I/O ticket with us. ");
      markup.AppendLine("     </p> ");
      markup.AppendLine("     <p> ");
      markup.AppendLine("       BOOKING DETAILS<br/> ");
      markup.AppendLine("       Reservation number: IO12345<br/> ");
      markup.AppendLine("       Order for: John Smith<br/> ");
      markup.AppendLine("       Event: Google I/O 2013<br/> ");
      markup.AppendLine("       Start time: May 15th 2013 8:00am PST<br/> ");
      markup.AppendLine("       Venue: Moscone Center, 800 Howard St., San Francisco, CA 94103<br/> ");
      markup.AppendLine("     </p> ");
      markup.AppendLine("   </body> ");
      markup.AppendLine(" </html> ");

      return markup.ToString();
    }


    #endregion

  }
}
