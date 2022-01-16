using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Dal.Cosmos
{
  public class UserDb : Db, IUserDb
  {
    public UserDb(string databaseId, string containerId, string endpoint, string primaryKey, string partitionKey) 
      : base(databaseId, containerId, endpoint, primaryKey, partitionKey)
    {
    }
  }
}
