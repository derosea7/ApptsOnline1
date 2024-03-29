﻿using Appts.Web.Ui.Scheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
      _logger = logger;
      _next = next;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
      try
      {
        await _next(httpContext);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Something went wrong: {ex}");
        
        // for now, i'll let default middleware handle this
        await HandleExceptionAsync(httpContext, ex);
      }
    }
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      //return context.Response.WriteAsync(new ErrorDetails()
      //{
      //  StatusCode = context.Response.StatusCode,
      //  Message = "Internal Server Error from the custom middleware."
      //}.ToString());
      //return context.Response.cont;
      return _next(context);
    }
  }
}
