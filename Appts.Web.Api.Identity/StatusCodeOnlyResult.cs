using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Identity
{
  public class StatusCodeOnlyResult : ActionResult
  {
    protected int StatusCode;

    public StatusCodeOnlyResult(int statusCode)
    {
      StatusCode = statusCode;
    }

    public override Task ExecuteResultAsync(ActionContext context)
    {
      context.HttpContext.Response.StatusCode = StatusCode;
      return base.ExecuteResultAsync(context);
    }
  }
}
