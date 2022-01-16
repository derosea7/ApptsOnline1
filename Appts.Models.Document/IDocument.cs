using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  /// <summary>
  /// Base interface for documents stored in schedules container.
  /// </summary>
  public interface IDocument
  {
    string Id { get; set; }
    string UserId { get; set; }
  }
}
