using System;
using Azure.Messaging.ServiceBus;
namespace Appts.Messaging.ServiceBus2
{
  public class Bus2
  {
    
    public Bus2()
    {
      
    }
    public string GetStockToWatch()
    {
      ServiceBusClient client = new ServiceBusClient("Endpoint=sb://schedules.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=RtapaUmEFamjryWDVCVZcdyGX7978zN9/UN7wdkcB3c=");
      //client.
      
      return "";
    }
  }
}
