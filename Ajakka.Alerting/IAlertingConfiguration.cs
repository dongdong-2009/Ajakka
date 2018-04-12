namespace Ajakka.Alerting{
    public interface IAlertingConfiguration{
        string MessageQueueHost {get;}

        string CommandProcessorRpcQueueName{get;}

        string EventListenerQueueName {get;}

        string MessageQueueUserName {get;}

        string MessageQueuePassword {get;}
    }
}