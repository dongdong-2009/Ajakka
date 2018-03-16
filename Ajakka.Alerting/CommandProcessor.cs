using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Alerting{
    public class CommandProcessor:IDisposable{

        IAlertingConfiguration configuration;
        IActionStore dal;
        IConnection connection;
        IModel channel;

        public CommandProcessor(IAlertingConfiguration configuration, IActionStore dal){
            if(configuration == null){
                throw new ArgumentNullException("configuration");
            }

            if(dal == null){
                throw new ArgumentNullException("dal");
            }

            this.configuration = configuration;
            this.dal = dal;
        }

        public void Start(){
            var factory = new ConnectionFactory() { HostName = configuration.MessageQueueHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        
            channel.QueueDeclare(queue: configuration.CommandProcessorRpcQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: configuration.CommandProcessorRpcQueueName, autoAck: false, consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                OnRequestReceived(ea, channel);
            };
    
        }

        private void OnRequestReceived(BasicDeliverEventArgs eventArgs, IModel channel){
            string response = null;

            var body = eventArgs.Body;
            var props = eventArgs.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                var message = Encoding.UTF8.GetString(body);
                var request = ParseRequest(message);
                if(string.IsNullOrEmpty(request.FunctionName)){
                    throw new InvalidDataException("Invalid request, FunctionName is empty.");
                };
                response = ProcessRequest(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var serverResponse = new CommandProcessorResponse<string>{
                    Error = true,
                    Message = ex.Message,
                    Content = ex.Message
                };
                response = SerializeResponse<CommandProcessorResponse<string>>(serverResponse);
            }
            finally
            {
                Console.WriteLine("Sending response: " + response);
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                basicProperties: replyProps, body: responseBytes);
                channel.BasicAck(deliveryTag: eventArgs.DeliveryTag,
                multiple: false);
            }
        }

        private dynamic ParseRequest(string message){
            var definition = new {FunctionName = "", ActionId = 0, PageNumber = 0, AlertMessage= "", ActionName="", ActionConfiguration="",ActionType=""};
            return JsonConvert.DeserializeAnonymousType(message, definition);
        }

        private string ProcessRequest(dynamic request){
            switch(request.FunctionName){
                case "GetAction":
                    return SerializeResponse<CommandProcessorResponse<AlertActionBase>>(GetAction(request.ActionId));
                case "AddAction":
                    return SerializeResponse<CommandProcessorResponse<AlertActionBase>>(AddAction(request.ActionName, request.ActionConfiguration, request.ActionType));
                case "GetPageCount":
                    return SerializeResponse<CommandProcessorResponse<int>>(GetPageCount());
                case "GetActionTypes":
                    return SerializeResponse<CommandProcessorResponse<ActionTypeDescriptor[]>>(GetActionTypes());
                default:
                    throw new InvalidOperationException("Function name not found: " +request.FunctionName);
            }
        }

        protected virtual CommandProcessorResponse<ActionTypeDescriptor[]> GetActionTypes(){
            return WrapResponse(AlertActionFactory.GetAlertActionTypes());
        }

        protected virtual CommandProcessorResponse<AlertActionBase[]> GetActions(int pageNumber){
            return WrapResponse(dal.GetActions(pageNumber));
        }

        protected virtual CommandProcessorResponse<AlertActionBase> GetAction(int actionId){
            return WrapResponse(dal.GetAction(actionId));
        }

        protected virtual CommandProcessorResponse<int> GetPageCount(){
            return WrapResponse(dal.GetPageCount());
        }

        protected virtual CommandProcessorResponse<AlertActionBase> AddAction(string name, string configuration, string type){
            var action = AlertActionFactory.Create(name,type,configuration);
            action.Initialize();
            return WrapResponse(dal.AddAction(action));
        }

        protected CommandProcessorResponse<T> WrapResponse<T>(T content){
            return new CommandProcessorResponse<T>(){Content = content};
        }

        protected string SerializeResponse<T>(T response){  
            var serializer = new DataContractJsonSerializer(response.GetType());
            using(var ms = new MemoryStream()){
                serializer.WriteObject(ms, response);  
                byte[] json = ms.ToArray();  
                ms.Close();  
                return Encoding.UTF8.GetString(json, 0, json.Length);  
            }  
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(connection != null){
                        connection.Dispose();
                    }
                    if(channel != null){
                        channel.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}