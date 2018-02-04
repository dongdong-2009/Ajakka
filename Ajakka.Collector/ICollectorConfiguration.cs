namespace Ajakka.Collector{
    public interface ICollectorConfiguration{
        string MessageQueueHost {get;}
       
        string MessageQueueExchangeName {get;}
    }
}