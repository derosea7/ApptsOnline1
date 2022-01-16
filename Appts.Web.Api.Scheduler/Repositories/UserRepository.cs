using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Docs;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly IUserDb _userDb;
    public UserRepository(IUserDb userDb)
    {
      _userDb = userDb;
    }
    public async Task Create(User user)
    {
      await _userDb.CreateNoReturnAsync2(user);
    }
  }
}
