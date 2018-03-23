using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Ajakka.TestSend
{
    public class Program
    {
        static  Configuration config = new Configuration();

        public static void Main(string[] args)
        {
            Console.WriteLine("Commands: " + Environment.NewLine + "send: sends notification about a new device" + Environment.NewLine + "Press Ctrl+C to exit");
            var command = "";
            do{
                command = Console.ReadLine();
                if(command.StartsWith("s"))
                {
                    ProcessSendCommand(command);
                }
            }while(command != "x");
        }

        static void ProcessSendCommand(string command){
            var parts = command.Split(' ');
            if(parts.Length == 1){
                var device = DeviceDescriptor.CreateRandom();
                SendNewDeviceNotification(device);
                Console.WriteLine("Sent new device notification: " + device);
            }
        }


        private static void SendNewDeviceNotification(DeviceDescriptor device)
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

        private static string BuildMessage(DeviceDescriptor device)
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
