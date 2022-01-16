﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Middleware
{
  public static class ExceptionMiddlewareExtensions
  {
    public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
    {
      app.UseMiddleware<ExceptionMiddleware>();
    }
  }
}
