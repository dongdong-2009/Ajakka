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
            else if(parts.Length == 2){
               device = CreateWithMac(parts[1]);
            }

            else if(parts.Length == 3){
                device = CreateWithMacIp(parts[1],parts[2]);
            }
            else if(parts.Length == 4){
                device = CreateWithAll(parts[1],parts[2],parts[3]);
            }
            else{
                Console.WriteLine("Invalid number of parameters. See help for usage information");
            }
            if(device != null){
                client.SendNewDeviceNotification(device);
                Console.WriteLine("Sent new device notification: " + device);
            }
        }

        DeviceDescriptor CreateWithMac(string mac){
            if(!ValidateMac(mac)){
                Console.WriteLine(mac + " is not a valid MAC.");
                return null;
            }
            return DeviceDescriptor.CreateRandom(mac);
        }

        DeviceDescriptor CreateWithMacIp(string mac, string ip){
            if(!ValidateMac(mac)){
                Console.WriteLine(mac + " is not a valid MAC.");
                return null;
            }
            if(!ValidateIp(ip)){
                Console.WriteLine(ip + " is not a valid IP.");
                return null;
            }
            return DeviceDescriptor.CreateRandom(mac, ip);
        }

        DeviceDescriptor CreateWithAll(string mac, string ip, string name){
            if(!ValidateMac(mac)){
                Console.WriteLine(mac + " is not a valid MAC.");
                return null;
            }
            if(!ValidateIp(ip)){
                Console.WriteLine(ip + " is not a valid IP.");
                return null;
            }
            if(!ValidateName(name)){
                Console.WriteLine(name + " is not a valid name");
                return null;
            }
            return new DeviceDescriptor(mac,ip,name);
        }

        private static bool ValidateName(string name){
            if(name.Length > 16)
                return false;
            foreach(var c in name){
                if (!(c >='a' && c<='z' || c >= 'A' && c <='Z' || c>='0' && c<='9' || c=='-')){
                    return false;
                }
            }
            return true;
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