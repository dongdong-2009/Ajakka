namespace Ajakka.Blacklist{
    public interface IBlacklistConfiguration{
        string MessageQueueHost {get;}
       
        string MessageQueueExchangeName {get;}

        string CommandProcessorRpcQueueName{get;}

        string AlertingRpcQueueName {get;}
        string AlertingEventQueueName{get;}
    }
}