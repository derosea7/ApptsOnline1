using Appts.Models.Document;
using Google.Apis.Auth.OAuth2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Services
{
  public interface IGoogleCalendarClient
  {
    string GetAuthorizationUrl();

    /// <summary>
    /// M
    /// </summary>
    /// <param name="serviceProviderId"></param>
    void Initialize(string serviceProviderId);
    Task<TokenResponse> ExchangeCodeForTokenAsync(string code);
    Task RevokeCalendarApiAccessForUserAsync();
    Task TryGetBearerToken();
    Task<string> GetEventsJsonAsync();
    Task<string> CreateCalendarEvent(Appointment appointment, string spVanityUrl);
    Task CancelCalendarEvent(string eventId, DateTime start, DateTime end);
  }
}
