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

        public DhcpSensor(IPEndPoint localEndpoint){
            Task.Run(async ()=>{
                await SensorLoop(localEndpoint);
            });
        }

        public void Stop()
        {
            stop = true;
        }

        private async Task SensorLoop(IPEndPoint localEndpoint){
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


    }
}