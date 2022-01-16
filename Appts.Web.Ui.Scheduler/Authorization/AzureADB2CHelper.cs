using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Authorization
{
  public class AzureADB2CHelper
  {
    private readonly Func<TokenValidatedContext, Task> _onTokenValidated;


    public AzureADB2CHelper(Func<TokenValidatedContext, Task> onTokenValidated)
    {
      _onTokenValidated = onTokenValidated;
    }


    public Task OnTokenValidated(TokenValidatedContext context)
    {
      _onTokenValidated?.Invoke(context);
      return Task.Run(async () =>
      {
        try
        {
          
          var oidClaim = context.Principal.Claims.FirstOrDefault(c => c.Type == "oid");
          ((ClaimsIdentity)context.Principal.Identity).AddClaim(new Claim(ClaimTypes.Role,
              "aldfkja", ClaimValueTypes.String));

        }
        catch (Exception e)
        {

        }
      });
    }
  }
}
