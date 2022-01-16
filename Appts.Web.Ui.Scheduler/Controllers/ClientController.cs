using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Messaging.ServiceBus;
using Appts.Models.Domain;
using Newtonsoft.Json;
using Appts.Models.View;
using Appts.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Appts.Models.Rest;
using Microsoft.Extensions.Logging;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Models.Document;
using Microsoft.AspNetCore.Mvc.Rendering;
using Appts.Web.Ui.Scheduler.ViewModels;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Support service provider management of their clients.
  /// </summary>
  //[Authorize("PaidSubscriber")]
  [Authorize]
  public class ClientController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly ILogger<ClientController> _logger;
    private readonly IBlobAvatarRepository _blobAvatarRepository;
    private TelemetryClient _telemetry;
    public ClientController(
      IApiClient apiClient, 
      ILogger<ClientController> logger, 
      IBlobAvatarRepository blobAvatarRepository,
      TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _logger = logger;
      _blobAvatarRepository = blobAvatarRepository;
      _telemetry = telemetry;
    }
    public IActionResult Index()
    {
      _telemetry.TrackPageView("ViewClientsAsSp");
      // call api and get servicep rovider
      // vantity url
      // display name?
      // client list -- does not exist right now.
      var userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var response = _apiClient.GetAsync<GetServiceProviderResponse>(
        $"/api/ServiceProvider/{userId}").GetAwaiter().GetResult();
      var clientlist = _apiClient.GetAsync<List<ClientDoc>>(
        $"/api/Client/GetClientDocs?spId={userId}").GetAwaiter().GetResult();
      var model = new ManageClientsViewModel()
      {
        ServiceProviderVanityUrl = response.VanityUrl,
        ServiceProviderDisplayName = response.DisplayName,
        RequestHost = Request.Host.ToString(),
        SpEmail = response.Email,
        Clients = clientlist
      };
      return View(model);
    }
    public IActionResult Add()
    {
      var model = new AddClientVm();
      model.StateOptions = GetStateOptions();
      return View(model);
    }
    [HttpPost]
    [ActionName("Add")]
    public IActionResult AddPost([FromForm] AddClientVm model)
    {
      // save

      TempData["UpdateSuccessful"] = Boolean.FalseString;
      return RedirectToAction("Add");
    }
    public IActionResult Invite()
    {
      _telemetry.TrackPageView("InviteClients");
      var userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var response = _apiClient.GetAsync<GetServiceProviderResponse>(
        $"/api/ServiceProvider/{userId}").GetAwaiter().GetResult();
      var model = new ManageClientsViewModel()
      {
        ServiceProviderVanityUrl = response.VanityUrl,
        ServiceProviderDisplayName = response.DisplayName,
        RequestHost = Request.Host.ToString(),
        SpEmail = response.Email
      };
      return View(model);
    }
    /// <summary>
    /// Sends a message to service bus, which triggers an azure function,
    /// which triggers an email be sent out; inviting client to schedule appointment
    /// with service provider.
    /// </summary>
    /// <param name="model">
    ///   Must contain email address to send invite to.
    /// </param>
    /// <returns>True if send is successfully</returns>
    public IActionResult SendInvite(ManageClientsViewModel model)
    {
      _telemetry.TrackEvent("SendInviteToClient");
      var request = new SendClientInvitationRequest()
      {
        ClientEmail = model.Email,
        ClientPhoneNumber = model.Phone,
        SpVanityUrl = $"https://{Request.Host}/schedule/{model.ServiceProviderVanityUrl}",
        SpDisplayName = model.ServiceProviderDisplayName,
        SpEmail = model.SpEmail,
        PersonalMessage = model.PersonalMessage,
        SendEmail = model.SendEmail,
        SendSms = model.SendSms
      };
      string endpoint = $"/api/Client/SendClientInvitationEmail";
      _apiClient.PostNoReturnAsync<SendClientInvitationRequest>(request, endpoint).GetAwaiter().GetResult();
      ViewData["inviteSent"] = true;
      return RedirectToAction("Invite", "Client");
    }

    public IActionResult Settings(string onboarding = null)
    {
      _telemetry.TrackPageView("ViewClientSettings");
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      try
      {
        if (onboarding == "t")
        {
          _blobAvatarRepository.CreateDefaultAvatar(userId);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed creating default avatar for user during onboarding.");
      }
      ClientDoc client = _apiClient.GetAsync<ClientDoc>(
        $"api/Client/GetClientDoc?id={userId}").GetAwaiter().GetResult();
      var vm = new ClientProfileVm()
      {
        ClientUserId = userId,
        IsOnboarding = onboarding == "t" ? true : false,
        MobilePhone = client.MobilePhone,
        Address = client.Address,
        Address2 = client.Address2,
        City = client.City,
        StateCode= client.StateCode,
        ZipCode = client.ZipCode,
        TimeZoneId = client.TimeZoneId
      };
      vm.StateOptions = GetStateOptions();
      return View(vm);
    }
    List<SelectListItem> GetStateOptions()
    {
      return new List<SelectListItem>()
      {
        new SelectListItem() { Value="-1", Text="Choose..." },
        new SelectListItem() { Value="AL", Text="Alabama" },
        new SelectListItem() { Value="AK", Text="Alaska" },
        new SelectListItem() { Value="AZ", Text="Arizona" },
        new SelectListItem() { Value="AR", Text="Arkansas" },
        new SelectListItem() { Value="CA", Text="California" },
        new SelectListItem() { Value="CO", Text="Colorado" },
        new SelectListItem() { Value="CT", Text="Connecticut" },
        new SelectListItem() { Value="DE", Text="Delaware" },
        new SelectListItem() { Value="DC", Text="District Of Columbia" },
        new SelectListItem() { Value="FL", Text="Florida" },
        new SelectListItem() { Value="GA", Text="Georgia" },
        new SelectListItem() { Value="HI", Text="Hawaii" },
        new SelectListItem() { Value="ID", Text="Idaho" },
        new SelectListItem() { Value="IL", Text="Illinois" },
        new SelectListItem() { Value="IN", Text="Indiana" },
        new SelectListItem() { Value="IA", Text="Iowa" },
        new SelectListItem() { Value="KS", Text="Kansas" },
        new SelectListItem() { Value="KY", Text="Kentucky" },
        new SelectListItem() { Value="LA", Text="Louisiana" },
        new SelectListItem() { Value="ME", Text="Maine" },
        new SelectListItem() { Value="MD", Text="Maryland" },
        new SelectListItem() { Value="MA", Text="Massachusetts" },
        new SelectListItem() { Value="MI", Text="Michigan" },
        new SelectListItem() { Value="MN", Text="Minnesota" },
        new SelectListItem() { Value="MS", Text="Mississippi" },
        new SelectListItem() { Value="MO", Text="Missouri" },
        new SelectListItem() { Value="MT", Text="Montana" },
        new SelectListItem() { Value="NE", Text="Nebraska" },
        new SelectListItem() { Value="NV", Text="Nevada" },
        new SelectListItem() { Value="NH", Text="New Hampshire" },
        new SelectListItem() { Value="NJ", Text="New Jersey" },
        new SelectListItem() { Value="NM", Text="New Mexico" },
        new SelectListItem() { Value="NY", Text="New York" },
        new SelectListItem() { Value="NC", Text="North Carolina" },
        new SelectListItem() { Value="ND", Text="North Dakota" },
        new SelectListItem() { Value="OH", Text="Ohio" },
        new SelectListItem() { Value="OK", Text="Oklahoma" },
        new SelectListItem() { Value="OR", Text="Oregon" },
        new SelectListItem() { Value="PA", Text="Pennsylvania" },
        new SelectListItem() { Value="RI", Text="Rhode Island" },
        new SelectListItem() { Value="SC", Text="South Carolina" },
        new SelectListItem() { Value="SD", Text="South Dakota" },
        new SelectListItem() { Value="TN", Text="Tennessee" },
        new SelectListItem() { Value="TX", Text="Texas" },
        new SelectListItem() { Value="UT", Text="Utah" },
        new SelectListItem() { Value="VT", Text="Vermont" },
        new SelectListItem() { Value="VA", Text="Virginia" },
        new SelectListItem() { Value="WA", Text="Washington" },
        new SelectListItem() { Value="WV", Text="West Virginia" },
        new SelectListItem() { Value="WI", Text="Wisconsin" },
        new SelectListItem() { Value="WY", Text="Wyoming" },
        new SelectListItem() { Value="0", Text="Not Listed" }
      };
    }
    [HttpPost]
    [ActionName("Settings")]
    public IActionResult SettingsPost(ClientProfileVm model)
    {
      _telemetry.TrackEvent("UpdateClientSettings");
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      ValidateAddressPartOfModel(model);
      if (ModelState.IsValid)
      {
        var request = new PatchClientSettingsRequest();
        ClientDoc updated = new ClientDoc()
        {
          UserId = userId,
          MobilePhone = model.MobilePhone,
          Address = model.Address,
          Address2 = model.Address2,
          City = model.City,
          StateCode = model.StateCode,
          ZipCode = model.ZipCode,
          TimeZoneId = model.TimeZoneId
        };
        request.UpdatedClient = updated;
        var response = _apiClient.PatchAsync<PatchClientSettingsRequest, PatchClientSettingsResponse>(
          request, $"api/Client/PatchClientSettings").GetAwaiter().GetResult();
        TempData["UpdateSuccessful"] = Boolean.TrueString;
        RedirectToAction("Settings");
      }
      return View(model);
    }
    void ValidateAddressPartOfModel(ClientProfileVm model)
    {
      if (model.Address != null || model.City != null || model.ZipCode != null || model.StateCode != null)
      {
        // make sure it is a valid combo of address
        if (model.Address == null || model.City == null || model.ZipCode == null || model.StateCode == null)
        {
          ModelState.AddModelError("InvalidAddress", "Invalid address");
        }
      }
    }
    [HttpPost]
    public bool UpdateTzId([FromBody] UpdateTimeZoneViewModel model)
    {
      _telemetry.TrackEvent("UpdateTzAsClient");
      bool wasUpdated = false;
      var request = new UpdateTimeZoneRequest()
      {
        ClientUserId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value,
        TimeZoneId = model.TimeZoneId
      };
      //_serviceProviderRepository.UpdateTzIdAsync(request).GetAwaiter().GetResult();
      _apiClient.PatchNoReturnAsync<UpdateTimeZoneRequest>(
        request, $"/api/Client/UpdateTzId").GetAwaiter().GetResult();
      wasUpdated = true; // if we made it here w/o ex
      return wasUpdated;
    }
    public IActionResult BecomeAServiceProvider()
    {
      return View();
    }
    [HttpPost]
    public IActionResult AddServiceProviderToClient()
    {
      var userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var email = User.Claims.FirstOrDefault(c => c.Type == IdentityK.Email).Value;
      var client = _apiClient.GetAsync<ClientDoc>($"api/Client/GetClientDoc?id={userId}").GetAwaiter().GetResult();
      var trial = new CreateTrialSubscriptionRequest()
      {
        ObjectId = userId,
        DisplayName = client.DisplayName,
        Email = email,
        GivenName = client.FirstName,
        Surname = client.LastName
      };
      var response = _apiClient.PostAsync<CreateTrialSubscriptionRequest, CreateTrialSubscriptionResponse>(
        trial, "api/Subscription/CreateTrialSubscription").GetAwaiter().GetResult();
      return RedirectToAction("Settings", "ServiceProvider", new { fromClient = "t" });
    }
  }
}
