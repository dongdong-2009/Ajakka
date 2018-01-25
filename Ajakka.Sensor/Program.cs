using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ajakka.Sensor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DhcpSensor sensor = null;
            Console.WriteLine("Sensor started");
            if(args.Length > 0){
                sensor = CreateSensorTryIp(args[0]);
            }
            if(sensor == null)
            {
                sensor = CreateSensorTryId(args[0]);
            }
            if(sensor == null){
                sensor = new DhcpSensor();
            }
            
            Console.ReadLine();
            sensor.Stop();
        }

        private static DhcpSensor CreateSensorTryId(string id){
            var endpoint = GetEndPointById(id);
            if(endpoint == null)
                return null;
            
            return new DhcpSensor(endpoint);
        }

        private static DhcpSensor CreateSensorTryIp(string localIp)
        {
            IPAddress ip;
            if(IPAddress.TryParse(localIp, out ip)){
                var endpoint = GetEndPointByIp(ip);
                if(endpoint == null)
                    return null;
                
                return new DhcpSensor(endpoint);
            }
            return null;
        }

        private static IPEndPoint GetEndPointByIp(IPAddress localIp)
        {
            foreach(var ni in NetworkInterface.GetAllNetworkInterfaces()){
                var prop = ni.GetIPProperties();
                foreach(var addr in prop.UnicastAddresses)
                {
                    if(addr.Address.Equals(localIp))
                        return new IPEndPoint(localIp, 67);
                }
            }
            Console.WriteLine("No interface found with IP " + localIp);
            return null;
        }

        private static IPEndPoint GetEndPointById(string id)
        {
            foreach(var ni in NetworkInterface.GetAllNetworkInterfaces()){
                if(ni.Id == id){
                    var prop = ni.GetIPProperties();
                    foreach(var addr in prop.UnicastAddresses)
                    {
                        if(addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            return new IPEndPoint(addr.Address, 67);
                    }
                }
            }
            Console.WriteLine("No interface found ipv4 IP and with id " + id);
            return null;
        }
    }
}
