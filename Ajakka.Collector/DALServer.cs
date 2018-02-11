using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Ajakka.Messaging;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;

namespace Ajakka.Collector{
    class DALServer:IDisposable{

        ICollectorConfiguration configuration;
        ICollectorDAL dal;
        IConnection connection;
        IModel channel;

        public DALServer(ICollectorConfiguration configuration, ICollectorDAL dal){
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
        
            channel.QueueDeclare(queue: configuration.DALServerRpcQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: configuration.DALServerRpcQueueName, autoAck: false, consumer: consumer);

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
                var serverResponse = new DALServerResponse<string>{
                    Error = true,
                    Message = ex.Message,
                    Content = ex.Message
                };
                response = SerializeResponse<DALServerResponse<string>>(serverResponse);
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
            var definition = new {FunctionName = "", PageNumber = 0, PageSize = 0};
            return JsonConvert.DeserializeAnonymousType(message, definition);
        }

        private string ProcessRequest(dynamic request){
            switch(request.FunctionName){
                case "GetLatest":
                    var endpoints = dal.GetEndpoints(request.PageNumber, request.PageSize);
                    var res1 = new DALServerResponse<EndpointDescriptor[]>{
                        Content = endpoints
                    };
                    return SerializeResponse<DALServerResponse<EndpointDescriptor[]>>(res1);

                case "GetDhcpEndpointPageCount":
                    var res2 = new DALServerResponse<int>{
                        Content = dal.GetDhcpEndpointPageCount(request.PageSize)
                    };
                    return SerializeResponse<DALServerResponse<int>>(res2);

                default:
                    throw new InvalidOperationException("Function name not found: " +request.FunctionName);
            }
        }

        private string SerializeResponse<T>(T response){  
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