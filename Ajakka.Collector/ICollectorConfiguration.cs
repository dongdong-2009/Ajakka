namespace Ajakka.Collector{
    public interface ICollectorConfiguration{
        string MessageQueueHost {get;}
       
        string MessageQueueExchangeName {get;}

        string DALServerRpcQueueName {get;}

        string MessageQueueUserName {get;}

        string MessageQueuePassword {get;}

    }
}