using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Appts.Messaging.ServiceBus
{
  public interface IBus
  {
    Task SendMessageAsync(string messageBodyJson);
    Task<long> ScheduleCancelableMessageAsync(string messageJson, DateTime sendOn);
    Task SendSmsMessageAsync(string messageBodyJson);
    Task<long> ScheduleCancelableSmsMessageAsync(string messageJson, DateTime sendOn);
    Task<bool> CancelScheduledSmsMessageAsync(long sequenceNumber);
    Task<int> CancelScheduledSmsMessagesAsync(List<long> sequenceNumbers);

    Task RecieveMessageAsync();
    Task SendStockToWatchMessageAsync(string messageBodyJson);
  }
}
