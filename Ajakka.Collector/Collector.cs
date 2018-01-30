using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Ajakka.Messaging;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Ajakka.Collector
{
    public class Collector
    {
        static void Main(string[] args)
        {
            var config = new CollectorConfiguration();

            var factory = new ConnectionFactory() { HostName = config.MessageQueueHost };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: config.MessageQueueExchangeName, type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                exchange: config.MessageQueueExchangeName,
                                routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    ProcessMessage(ea.Body);
                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static void ProcessMessage(byte[] body)
        {
            try{
                var deviceDescriptor = ParseMesage(body);
                StoreDeviceInfo(deviceDescriptor);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not parse the message: " + ex);
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" Message:{0}{1}", Environment.NewLine, message);
            }
        }
        static DeviceDescriptorMessage ParseMesage(byte[] message){
            using(var stream = new MemoryStream(message)){
                var serializer = new DataContractJsonSerializer(typeof(DeviceDescriptorMessage));
                return (DeviceDescriptorMessage)serializer.ReadObject(stream);
            }
        }

        static void StoreDeviceInfo(DeviceDescriptorMessage deviceInfo){
            ICollectorDAL dal = new DAL();
            dal.StoreDhcpEndpoint(deviceInfo.DeviceMacAddress, deviceInfo.DeviceIpAddress, deviceInfo.DeviceName);
        }
    }
}
