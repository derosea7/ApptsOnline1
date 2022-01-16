using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Docs;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IUserRepository
  {
    Task Create(User user);
  }
}
