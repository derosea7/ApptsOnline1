using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class OrganizationRepository : IOrganizationRepository
  {
    private readonly IApiClient _apiClient;
    public OrganizationRepository(IApiClient apiClient)
    {
      _apiClient = apiClient;
    }

    public async Task<string[]> GetTestValuesAsync()
    {
      return await _apiClient.GetAsync<string[]>(
        $"/api/values");
    }
  }
}
