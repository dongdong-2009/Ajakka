using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;

namespace Ajakka.Blacklist{
    public class CommandProcessor:IDisposable{

        IBlacklistConfiguration configuration;
        IBlacklist dal;
        IConnection connection;
        IModel channel;

        public CommandProcessor(IBlacklistConfiguration configuration, IBlacklist dal){
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
            try{
                var definition = new {FunctionName = "", PageNumber = 0, PageSize = 0, RuleId = Guid.Empty, RuleName = "", RulePattern = "", ActionId = 0};
                return JsonConvert.DeserializeAnonymousType(message, definition);
            }
            catch(Exception ex){
                Console.WriteLine("Failed to parse message: " + ex);
                Console.WriteLine("Message: " + message);
                throw ex;
            }
        }

        private string ProcessRequest(dynamic request){
            switch(request.FunctionName){
                case "GetRules":
                    return SerializeResponse<CommandProcessorResponse<Rule[]>>(GetRules(request.PageNumber));
                case "GetRule":
                    return SerializeResponse<CommandProcessorResponse<Rule>>(GetRule(request.RuleId));
                case "AddRule":
                    return SerializeResponse<CommandProcessorResponse<Rule>>(AddRule(request.RuleName, request.RulePattern));
                case "LinkAction":
                    return SerializeResponse<CommandProcessorResponse<Rule>>(LinkAction(request.RuleId, request.ActionId));
                case "GetPageCount":
                    return SerializeResponse<CommandProcessorResponse<int>>(GetPageCount());
                default:
                    throw new InvalidOperationException("Function name not found: " +request.FunctionName);
            }
        }

        protected virtual CommandProcessorResponse<Rule> LinkAction(Guid ruleId, int actionId){
            var rule = dal.GetRule(ruleId);
            rule.AlertActionIds.Add(actionId);
            return WrapResponse(dal.UpdateRule(rule.Id, rule));
        }

        protected virtual CommandProcessorResponse<Rule[]> GetRules(int PageNumber){
            return WrapResponse(dal.GetRules(PageNumber));
        }

        protected virtual CommandProcessorResponse<Rule> GetRule(Guid ruleId){
            return WrapResponse(dal.GetRule(ruleId));
        }

        protected virtual CommandProcessorResponse<int> GetPageCount(){
            return WrapResponse(dal.GetRulePageCount());
        }

        protected virtual CommandProcessorResponse<Rule> AddRule(string name, string pattern){
            var r = new Rule(){
                Name = name,
                Pattern = pattern
            };
            return WrapResponse(dal.AddRule(r));
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