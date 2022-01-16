using Appts.Common.Constants;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Web.Ui.Scheduler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Authorization
{
  public class SchedulingPrivacyLevelHandler : AuthorizationHandler<ScheduleAppointmentRequirement>
  {

    private readonly IServiceProviderRepository _serviceProviderRepository;
    public SchedulingPrivacyLevelHandler(IServiceProviderRepository serviceProviderRepository)
    {
      _serviceProviderRepository = serviceProviderRepository;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScheduleAppointmentRequirement requirement)
    {
      // write code to define whether or not the requirement is met.

      // filter context contains a lot of information
      // can get the model state, http context, route state
      var filterContext = context.Resource as AuthorizationFilterContext;
      if (filterContext == null)
      {
        context.Fail();
        return Task.CompletedTask;
      }

      var spBusinessNameUrl = filterContext.RouteData.Values["serviceProviderVanityUrl"].ToString();
      
      GetSchedulingPrivacyLevelResponse response = _serviceProviderRepository.GetSchedulingPrivacyLevelAsync(spBusinessNameUrl)
        .GetAwaiter().GetResult();

      // use will receieve message on the scheduling page
      // this is not a failed authentication, it is a failed query by user
      if (!response.FoundServiceProvider)
      {
        context.Succeed(requirement);
        return Task.CompletedTask;
      }

      SchedulingPrivacyLevel privacy = response.SchedulingPrivacyLevel;

      switch (privacy)
      {
        case SchedulingPrivacyLevel.AllowAnonymous:
          context.Succeed(requirement);
          return Task.CompletedTask;

        case SchedulingPrivacyLevel.AllowWhitelisted:
          if (filterContext.HttpContext.User.Identity.IsAuthenticated)
          {
            string email = filterContext.HttpContext.User.Claims.First(c => c.Type == IdentityK.Email).Value;

            // the business user is the user
            if (response.ServiceProviderEmail == email)
            {
              context.Succeed(requirement);
              return Task.CompletedTask;
            }

            bool userEmailIsWhitelisted = false;
            userEmailIsWhitelisted = _serviceProviderRepository
              .IsEmailWhitelisted(response.ServiceProviderId, email)
              .GetAwaiter().GetResult();

            if (userEmailIsWhitelisted)
            {
              context.Succeed(requirement);
              return Task.CompletedTask;
            }
          }

          break;
        case SchedulingPrivacyLevel.AllowAnyRegistered:
          if (filterContext.HttpContext.User.Identity.IsAuthenticated)
          {
            context.Succeed(requirement);
            return Task.CompletedTask;
          }

          break;
      }

      context.Fail();
      return Task.CompletedTask;
    }
  }
}
