using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Ajakka.Net;
using RabbitMQ.Client;
using Ajakka.Messaging;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Ajakka.Sensor{
    class DhcpSensor{

        bool stop = false;

        public DhcpSensor(SensorConfiguration configuration){
            bool valid = ValidateAndLogConfiguration(configuration);
            if(!valid){
                Console.WriteLine("Configuration is not valid. Sensor cannot start.");
                return;
            }
            Task.Run(async ()=>{
                await SensorLoop(configuration);
            });
        }

        public void Stop()
        {
            stop = true;
        }

        private bool ValidateAndLogConfiguration(SensorConfiguration configuration){
            Console.WriteLine("queueName: " + configuration.QueueName);
            if(string.IsNullOrEmpty(configuration.QueueName))
            {
                return false;
            }
            Console.WriteLine("messageQueueRoutingKey: " + configuration.MessageQueueRoutingKey);
            if(string.IsNullOrEmpty(configuration.MessageQueueRoutingKey))
            {
                return false;
            }

            Console.WriteLine("messageQueueExchangeName: " + configuration.MessageQueueExchangeName);

            Console.WriteLine("messageQueueHost: " + configuration.MessageQueueHost);
            if(string.IsNullOrEmpty(configuration.MessageQueueHost))
            {
                Console.WriteLine("messageQueueHost address empty, using localhost");
            }
            Console.WriteLine("enableMessaging: " + configuration.EnableMessaging);
            return true;
        }

        private async Task SensorLoop(SensorConfiguration configuration){
            try{
                
                using (var udpClient = new UdpClient(new IPEndPoint(0,67))){
                    
                    udpClient.EnableBroadcast = true;
                    Console.WriteLine("Starting to listen on " + udpClient.Client.LocalEndPoint);
                    while (!stop)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        var packet = new Ajakka.Net.DhcpPacket(receivedResults.Buffer);
                        Console.WriteLine("Received packet. Actual DHCP: " + packet.IsActualDhcp);
                        Console.WriteLine("MAC: " + packet.GetClientMac());
                        Console.WriteLine("Hostname: " + packet.GetHostName());
                        if(configuration.EnableMessaging)
                        {
                            SendNotification(configuration, packet);
                        }
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        private void SendNotification(SensorConfiguration configuration, DhcpPacket packet)
        {
            var factory = new ConnectionFactory() { 
                HostName = string.IsNullOrEmpty(configuration.MessageQueueHost) ? 
                    "localhost" :
                    configuration.MessageQueueHost 
            };
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: configuration.QueueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    string message = BuildMessage(packet);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: string.IsNullOrEmpty(configuration.MessageQueueExchangeName) ? "": configuration.MessageQueueExchangeName,
                                        routingKey: configuration.MessageQueueRoutingKey,
                                        basicProperties: null,
                                        body: body);
                }
            }
        }

        private string BuildMessage(DhcpPacket packet)
        {
            var deviceName = packet.GetHostName();
            var ip = packet.GetClientIp();
            var mac = packet.GetClientMac();

            var message = new DeviceDescriptorMessage
            {
                DeviceName = deviceName,
                DeviceIpAddress = ip == null ? string.Empty: ip.ToString(),
                DeviceMacAddress = mac == null ? string.Empty : mac.ToString(),
                TimeStamp = DateTime.UtcNow
            };
            var ms = new MemoryStream();  

            // Serializer the User object to the stream.  
            var serializer = new DataContractJsonSerializer(typeof(DeviceDescriptorMessage));  
            serializer.WriteObject(ms, message);  
            byte[] json = ms.ToArray();  
            ms.Close();  
            return Encoding.UTF8.GetString(json, 0, json.Length);  
        }



    }
}