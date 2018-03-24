using System;
using System.Net;

namespace Ajakka.TestSend{
    class SendCommandProcessor{
        
        RabbitMqClient client = new RabbitMqClient();

        public void ProcessSendCommand(string command){
            DeviceDescriptor device = null;
            var parts = command.Split(' ');
            if(parts.Length == 1){
                device = DeviceDescriptor.CreateRandom();
            }

            if(parts.Length == 2){
                if(!ValidateMac(parts[1])){
                    Console.WriteLine(parts[1] + " is not a valid MAC.");
                    return;
                }
                device = DeviceDescriptor.CreateRandom(parts[1]);
            }

            if(parts.Length == 3){
                if(!ValidateMac(parts[1])){
                    Console.WriteLine(parts[1] + " is not a valid MAC.");
                    return;
                }
                if(!ValidateIp(parts[2])){
                    Console.WriteLine(parts[2] + " is not a valid IP.");
                    return;
                }
                device = DeviceDescriptor.CreateRandom(parts[1], parts[2]);
            }
            if(device != null){
                client.SendNewDeviceNotification(device);
                Console.WriteLine("Sent new device notification: " + device);
            }
        }

         private static bool ValidateIp(string ip){
            try{
                IPAddress.Parse(ip);
                return true;
            }
            catch(Exception)
            {
                return false;
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
    }
}