using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Appts.Web.Api.Scheduler.Repositories;
using Appts.Messaging.ServiceBus;
using Newtonsoft.Json;
using Appts.Models.SendGrid;
using Appts.Models.SendGrid.Templates;
using Appts.Models.Docs;
using Appts.Models.Document;
using Appts.Web.Api.Scheduler.Services;
using Appts.Models.SendGrid.Requests;

namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  [Route("api/[controller]/[action]")]
  public class ClientController : Controller
  {
    private readonly IClientRepository _clientRepository;
    private readonly ICommunicationService _comm;
    public ClientController(IClientRepository clientRepository, ICommunicationService comm)
    {
      _clientRepository = clientRepository;
      _comm = comm;
    }

    //create only if does not exist
    [HttpPost]
    public CreateClientResponse Create([FromBody]CreateClientRequest request)
    {
      // sned thank you email for signing up here
      try
      {
        SendThankYouForJoiningEmailAsync(request.DisplayName, request.Email).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        //log
        //throw;
      }
      return _clientRepository.CreateIfNotExistsAsync(request).GetAwaiter().GetResult();
    }
    [HttpPatch]
    public void UpdateTzId([FromBody] UpdateTimeZoneRequest request)
    {
      _clientRepository.UpdateTzIdAsync(request).GetAwaiter().GetResult();
    }
    [HttpPatch]
    public PatchClientSettingsResponse PatchClientSettings([FromBody] PatchClientSettingsRequest request)
    {
      return _clientRepository.PatchClientSettingsAsync(request).GetAwaiter().GetResult();
    }
    //[HttpGet("api/[controller]/[action]/{id}")]
    [HttpGet]
    public ClientDoc GetClientDoc(string id)
    {
      return _clientRepository.GetClientAsync(id).GetAwaiter().GetResult();
    }
    [HttpGet]
    public  List<string> GetClientIdList(string spId)
    {
      return _clientRepository.GetClientIdListAsync(spId).GetAwaiter().GetResult();
    }
    [HttpGet]
    public List<ClientDoc> GetClientDocs(string spId)
    {
      var clientIdList = _clientRepository.GetClientIdListAsync(spId).GetAwaiter().GetResult();
      return _clientRepository.GetClientsAsync(spId, clientIdList).GetAwaiter().GetResult();
    }
    [HttpPost]
    public void SendClientInvitationEmail([FromBody]SendClientInvitationRequest request)
    {
      var request2 = new SendClientInviteRequest()
      {
        ClientEmail = request.ClientEmail,
        ClientPhoneNumber = request.ClientPhoneNumber,
        PersonalMessageFromSp = request.PersonalMessage,
        VanityUrl = request.SpVanityUrl,
        SpDisplayName = request.SpDisplayName,
        SpEmail = request.SpEmail,
        SendEmail = request.SendEmail,
        SendSms = request.SendSms
      };
      _comm.SendClientInviteAsync(request2);
    }
    async Task<bool> SendThankYouForJoiningEmailAsync(string firstName, string email)
    {
      var request = new SendWelcomeRequest()
      {
        CustomerEmail = email,
        CustomerFirstName = firstName,
        CustomerLastName = "last name"
      };
      return await _comm.SendThankYouForJoiningEmailAsync(request);
    }
    /// <summary>
    /// Get the json that will be sent in Service Bus Message to trigger
    /// Azure Function to send SendMail HTTP Post request to SendGrid,
    /// which ultimately send email template with dynamic data
    /// </summary>
    /// <param name="request">This object will contain the details of where to send email and include dynamic template data.</param>
    /// <returns>string of Json that can be sent in body of Service Bus message.</returns>
  }
}
