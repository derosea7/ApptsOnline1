using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Appts.Web.Api.Identity
{
  public class BasicAuthenticationFilterAttribute : Attribute, IAsyncAuthorizationFilter
  {
    public string Realm { get; set; }
    public const string AuthTypeName = "Basic ";
    private const string _authHeaderName = "Authorization";
    private IConfiguration _config;

    public BasicAuthenticationFilterAttribute(string realm = null)
    {
      Realm = realm;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
      try
      {
        _config = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
        var request = context?.HttpContext?.Request;
        var authHeader = request.Headers.Keys.Contains(_authHeaderName) ? request.Headers[_authHeaderName].First() : null;
        string encodedAuth = (authHeader != null && authHeader.StartsWith(AuthTypeName)) ? authHeader.Substring(AuthTypeName.Length).Trim() : null;
        if (string.IsNullOrEmpty(encodedAuth))
        {
          context.Result = new BasicAuthChallengeResult(Realm);
          return;
        }

        var (username, password) = DecodeUserIdAndPassword(encodedAuth);

        if (username != _config["Ief:Username"] || password != _config["Ief:Password"])
        {
          context.Result = new StatusCodeOnlyResult(StatusCodes.Status401Unauthorized);
        }

        // Populate user: adjust claims as needed
        var claims = new[] { new Claim(ClaimTypes.Name, username, ClaimValueTypes.String, AuthTypeName) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthTypeName));
        context.HttpContext.User = principal;
      }
      catch
      {
        // log and reject
        context.Result = new StatusCodeOnlyResult(StatusCodes.Status401Unauthorized);
      }
    }

    private static (string userid, string password) DecodeUserIdAndPassword(string encodedAuth)
    {
      var userpass = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuth));
      var separator = userpass.IndexOf(':');
      if (separator == -1)
        return (null, null);

      return (userpass.Substring(0, separator), userpass.Substring(separator + 1));
    }
  }
}
