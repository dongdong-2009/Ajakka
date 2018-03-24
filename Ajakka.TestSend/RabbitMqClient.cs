using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Ajakka.TestSend{
    class RabbitMqClient{
        Configuration config = new Configuration();

        public void SendNewDeviceNotification(DeviceDescriptor device)
        {
            var factory = new ConnectionFactory() { 
                HostName = string.IsNullOrEmpty(config.MessageQueueHost) ? 
                    "localhost" :
                    config.MessageQueueHost 
            };
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: config.MessageQueueExchangeName, type: "fanout");
                    
                    string message = BuildMessage(device);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: string.IsNullOrEmpty(config.MessageQueueExchangeName) ? "": config.MessageQueueExchangeName,
                                        routingKey: "",
                                        basicProperties: null,
                                        body: body);
                }
            }
        }

        private string BuildMessage(DeviceDescriptor device)
        {
            var message = new 
            {
                DeviceName = device.Name,
                DeviceIpAddress = device.Ip == null ? string.Empty: device.Ip,
                DeviceMacAddress = device.Mac == null ? string.Empty : device.Mac.ToString(),
                TimeStamp = DateTime.UtcNow
            };
            return JsonConvert.SerializeObject(message); 
        }

    }
}