using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Alerting{
    public class AlertEventProcessor:IDisposable{
        IConnection connection;
        IModel channel;
        IAlertingConfiguration configuration;
        IActionStore dal;
        
        public AlertEventProcessor(IAlertingConfiguration config, IActionStore dal){
            this.dal = dal;
            configuration = config;
            var factory = new ConnectionFactory() { HostName = configuration.MessageQueueHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

             channel.QueueDeclare(queue: config.EventListenerQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
           
            consumer.Received += (source, ea) =>{
                OnRequestReceived( ea);
            };

            channel.BasicConsume(queue: config.EventListenerQueueName,
                                 autoAck: false,
                                 consumer: consumer);
        }

        private void OnRequestReceived(BasicDeliverEventArgs eventArgs){
           
            var body = eventArgs.Body;
            var message = Encoding.UTF8.GetString(body);

            var request = ParseRequest(message);
            try{
                ProcessRequest(request);
            }
            catch(Exception ex){
                Console.WriteLine("Failed to process request: " + ex);
            }
            channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
            
        }

        void ProcessRequest(dynamic request){
            switch(request.MessageType){
                case "BlacklistMatch":
                    Execute(request.RuleId, request.Mac, request.Ip, request.Name);
                    break;
                case "RuleDeleted":
                    OnRuleDeleted(request.RuleId);
                break;
                default:
                    throw new InvalidOperationException("Command does not exist: " + request.FunctionName);
            }
        }

        protected virtual void OnRuleDeleted(dynamic ruleId)
        {
            dal.DeleteRuleAndActions(ruleId);
        }

        protected virtual void Execute(Guid ruleId, string mac, string ip, string name){
            var linkedActions = dal.GetLinkedActions(ruleId);
            foreach(var action in linkedActions){
                action.Execute( new{RuleId = ruleId, Mac = mac, Ip = ip, Name = name});
            }
        }
        
        private dynamic ParseRequest(string message){
            var definition = new {MessageType = "", RuleId = Guid.Empty, Mac = "", Ip = "", Name = ""};
            return JsonConvert.DeserializeAnonymousType(message, definition);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channel.Dispose();
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}