using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.SendGrid
{
  public class UnsubscribeGroup
  {
    [JsonProperty("group_id")]
    public int GroupId { get; set; }
    [JsonProperty("groups_to_display")]
    public List<int> GroupsToDisplay { get; set; }
  }
}
