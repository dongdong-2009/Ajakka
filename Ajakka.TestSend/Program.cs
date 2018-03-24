using System;
using System.IO;
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
            ShowHelp();
            var command = "";
            do{
                command = Console.ReadLine();
                if(command.ToLower().StartsWith("s"))
                {
                    ProcessSendCommand(command);
                }
                if(command.ToLower().StartsWith("h")){
                    ShowHelp();
                }
                if(command.ToLower().StartsWith("c")){
                    Console.Clear();
                }
            }while(command.ToLower() != "x");
        }

        static void ShowHelp(){
            using(var reader = new StreamReader("help.txt")){
                Console.WriteLine(reader.ReadToEnd());
            }
        }
        static void ProcessSendCommand(string command){
            var parts = command.Split(' ');
            if(parts.Length == 1){
                var device = DeviceDescriptor.CreateRandom();
                SendNewDeviceNotification(device);
                Console.WriteLine("Sent new device notification: " + device);
            }

            if(parts.Length == 2){
                if(!ValidateMac(parts[1])){
                    Console.WriteLine(parts[1] + " is not a valid MAC.");
                    return;
                }
                var device = DeviceDescriptor.CreateRandom();
                device.Mac = parts[1].ToUpper();
                SendNewDeviceNotification(device);
                Console.WriteLine("Sent new device notification: " + device);
            }
        }

        private static bool ValidateMac(string mac){
            try{
                var number = Convert.ToInt64(mac,16);
                return mac.Length == 12;
            }
            catch(Exception){
                return false;
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
