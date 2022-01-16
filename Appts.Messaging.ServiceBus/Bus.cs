using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Appts.Models.Domain;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Threading;

//using Azure.Messaging.ServiceBus

namespace Appts.Messaging.ServiceBus
{
  /// <summary>
  /// Connections are pooled with ServiceBusConnection, and this call
  /// must be registered as a singleton to utilize pooling.
  /// </summary>
  public class Bus : IBus
  {
    private static ServiceBusConnection _connection;
    private static string _emailQ;
    private static string _smsQ;
    private static QueueClient _stocksQ;
    private static Message _stockMessage;
    private static string _stockBody;
    public Bus(IConfiguration config)
    {
      string sbConnectionString = config["ServiceBus:ConnectionString"].ToString();
      _emailQ = config["ServiceBus:EmailQueueName"].ToString();
      _smsQ = config["ServiceBus:SmsQueueName"].ToString();
      _connection = new ServiceBusConnection(sbConnectionString);
    }
    public async Task SendMessageBaseAsync(string messageBodyJson, string qName)
    {
      try
      {
        var qClient = new QueueClient(_connection, qName, ReceiveMode.PeekLock, RetryPolicy.Default);
        var msg = new Message(Encoding.UTF8.GetBytes(messageBodyJson));
        await qClient.SendAsync(msg);
      }
      catch (Exception ex)
      {
        //TODO: handle exception
        //throw;
      }
    }
    public async Task<long> ScheduleCancelableMessageBaseAsync(string messageJson, string qName, DateTime sendOn)
    {
      long sequenceNumber = -1;
      try
      {
        var qClient = new QueueClient(_connection, qName, ReceiveMode.PeekLock, RetryPolicy.Default);
        var message = new Message(Encoding.UTF8.GetBytes(messageJson));
        sequenceNumber = await qClient.ScheduleMessageAsync(message, sendOn).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        //TODO: handle exception
        //throw;
      }
      return sequenceNumber;
    }
    public async Task<bool> CancelScheduledMessageBaseAsync(string qName, long sequenceNumber)
    {
      bool success = false;
      var qClient = new QueueClient(_connection, qName, ReceiveMode.PeekLock, RetryPolicy.Default);
      await qClient.CancelScheduledMessageAsync(sequenceNumber);
      return success;
    }
    public async Task<int> CancelScheduledMessagesBaseAsync(string qName, List<long> sequenceNumbers)
    {
      int canceledCout = 0;
      var qClient = new QueueClient(_connection, qName, ReceiveMode.PeekLock, RetryPolicy.Default);
      foreach (long sequenceNumber in sequenceNumbers)
      {
        await qClient.CancelScheduledMessageAsync(sequenceNumber);
        canceledCout++;
      }
      return canceledCout;
    }
    #region "Emails"
    public async Task SendMessageAsync(string messageBodyJson)
    {
      await SendMessageBaseAsync(messageBodyJson, _emailQ);
    }
    public async Task<long> ScheduleCancelableMessageAsync(string messageJson, DateTime sendOn)
    {
      return await ScheduleCancelableMessageBaseAsync(messageJson, _emailQ, sendOn);
    }
    public async Task<bool> CancelScheduledMessageAsync(long sequenceNumber)
    {
      return await CancelScheduledMessageBaseAsync(_emailQ, sequenceNumber);
    }
    #endregion
    #region "Sms"
    public async Task SendSmsMessageAsync(string messageBodyJson)
    {
      await SendMessageBaseAsync(messageBodyJson, _smsQ);
    }
    public async Task<long> ScheduleCancelableSmsMessageAsync(string messageJson, DateTime sendOn)
    {
      return await ScheduleCancelableMessageBaseAsync(messageJson, _smsQ, sendOn);
      //return 1;
    }
    public async Task<bool> CancelScheduledSmsMessageAsync(long sequenceNumber)
    {
      return await CancelScheduledMessageBaseAsync(_smsQ, sequenceNumber);
      //return true;
    }
    public async Task<int> CancelScheduledSmsMessagesAsync(List<long> sequenceNumbers)
    {
      return await CancelScheduledMessagesBaseAsync(_smsQ, sequenceNumbers);
      //return 1;
    }
    #endregion
    #region "Stock"
    public async Task SendStockToWatchMessageAsync(string messageBodyJson)
    {
      await SendMessageBaseAsync(messageBodyJson, "stocks-to-watch");
    }
    public async Task RecieveMessageAsync()
    {
      _stocksQ = new QueueClient(_connection, "stocks-to-watch", ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);
      await HandleQueue();

    }
    static async Task HandleQueue()
    {
      var handler = new MessageHandlerOptions(ErrorHandler)
      {
        MaxConcurrentCalls = 10,
        AutoComplete = false
      };
      _stocksQ.RegisterMessageHandler(ProcessMessagesAsync, handler);
    }
    static Task ErrorHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
      return Task.CompletedTask;
    }
    static async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
      _stockMessage = message;
      _stockBody = Encoding.UTF8.GetString(message.Body);
      //Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
      string msg = $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}";
      await _stocksQ.CompleteAsync(message.SystemProperties.LockToken);
    }
    #endregion
  }
}