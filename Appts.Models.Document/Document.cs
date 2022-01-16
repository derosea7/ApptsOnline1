using Newtonsoft.Json;
using System;

namespace Appts.Models.Document
{
  /// <summary>
  /// Base class for all documents stored in schedules container.
  /// </summary>
  public class Document : IDocument
  {
    /// <summary>
    /// Unique identifier for the record. Random guid for now.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Way to seperate different docuemnt types.
    /// Schedules, availability, appts, ect.
    /// </summary>
    [JsonProperty(PropertyName = "entityType")]
    public string EntityType { get; set; }

    /// <summary>
    /// The schedules container partition key.
    /// </summary>
    [JsonProperty(PropertyName = "userId")]
    public string UserId { get; set; }

  }
}
