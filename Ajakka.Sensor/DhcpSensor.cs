using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Ajakka.Net;

namespace Ajakka.Sensor{
    class DhcpSensor{

        bool stop = false;

        public DhcpSensor():this(null)
        {
            
        }

        public DhcpSensor(SensorConfiguration configuration){
            Task.Run(async ()=>{
                await SensorLoop(configuration);
            });
        }

        public void Stop()
        {
            stop = true;
        }

        private async Task SensorLoop(SensorConfiguration configuration){
            var localEndpoint = GetSensorEndpoint(configuration);

            try{
                
                using (var udpClient = GetClient(localEndpoint)){
                    
                    udpClient.EnableBroadcast = true;
                    Console.WriteLine("Starting to listen on " + udpClient.Client.LocalEndPoint);
                    while (!stop)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        var packet = new Ajakka.Net.DhcpPacket(receivedResults.Buffer);
                        Console.WriteLine("Received packet. Actual DHCP: " + packet.IsActualDhcp);
                        Console.WriteLine("MAC: " + packet.GetClientMac());
                        Console.WriteLine("Hostname: " + packet.GetHostName());
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        private UdpClient GetClient(IPEndPoint localEndpoint)
        {
            if(localEndpoint != null){
                return new UdpClient(localEndpoint);
            }
            return new UdpClient(67);
        }

        private static IPEndPoint GetEndPointByIpString(string localIp)
        {
            IPAddress ip;
            if(IPAddress.TryParse(localIp, out ip)){
                var endpoint = GetEndPointByIp(ip);
                return endpoint;
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

        private static IPEndPoint GetSensorEndpoint(SensorConfiguration configuration){
            IPEndPoint endPoint = null;
            if(!string.IsNullOrEmpty(configuration.IpAddress)){
                endPoint = GetEndPointByIpString(configuration.IpAddress);
            }
            if(endPoint == null && !string.IsNullOrEmpty(configuration.InterfaceId)){
                endPoint = GetEndPointById(configuration.InterfaceId);
            }
            if(endPoint == null){
                endPoint = new IPEndPoint(0,67);
            }
            return endPoint;
        }
    }
}