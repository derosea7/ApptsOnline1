using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class ApptTelemetryRepository : IApptTelemetryRepository
  {
    private readonly IApiClient _apiClient;
    private readonly ILogger<ApptTelemetryRepository> _logger;
    public ApptTelemetryRepository(
      IApiClient apiClient, 
      ILogger<ApptTelemetryRepository> logger)
    {
      _apiClient = apiClient;
      _logger = logger;
    }
    
  }
}