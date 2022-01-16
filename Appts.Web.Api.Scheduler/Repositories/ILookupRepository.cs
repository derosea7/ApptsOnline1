using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface ILookupRepository
  {
    Task<string> GetAsync(string lookupPartition, string key);
    Task PostAsync(string lookupPartition, string key, string value);
    Task PatchAsync(string lookupPartition, string key, string newValue = null, string newKey = null);
  }
}
