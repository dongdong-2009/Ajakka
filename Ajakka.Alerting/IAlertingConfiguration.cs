namespace Ajakka.Alerting{
    public interface IAlertingConfiguration{
        string MessageQueueHost {get;}

        string CommandProcessorRpcQueueName{get;}
    }
}