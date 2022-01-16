using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  /// <summary>
  /// Container for long sequence number returned by Service Bus.
  /// Default is -1. If sequence number = -1, nothing was assigned
  /// or there was an error.
  /// </summary>
  public class CommunicationResponse
  {
    public long EmailSequenceNumber { get; set; }
    public long SmsSequenceNumber { get; set; }
    public CommunicationResponse()
    {
      EmailSequenceNumber = -1;
      SmsSequenceNumber = -1;
    }
    //public CommunicationResponse(long emailSequenceNumber, long smsSequenceNumber)
    //{
    //  EmailSequenceNumber = emailSequenceNumber;
    //  SmsSequenceNumber = smsSequenceNumber;
    //}
  }
}
