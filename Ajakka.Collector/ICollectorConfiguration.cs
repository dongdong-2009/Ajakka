namespace Ajakka.Collector{
    interface ICollectorConfiguration{
        string MessageQueueHost {get;}
       
        string MessageQueueExchangeName {get;}
    }
}