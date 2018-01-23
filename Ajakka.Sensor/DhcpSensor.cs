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

        public DhcpSensor()
        {
            Task.Run(async ()=>{
                await SensorLoop();
            });
        
        }

        public void Stop()
        {
            stop = true;
        }

        private async Task SensorLoop(){
            try{
                using (var udpClient = new UdpClient(67)){
                    
                    udpClient.EnableBroadcast = true;
                    //string loggingEvent = "";
                    Console.WriteLine("Starting to listen on " + udpClient.Client.LocalEndPoint);
                    while (!stop)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        var packet = new Ajakka.Net.DhcpPacket(receivedResults.Buffer);
                        Console.WriteLine("Received packet. Actual DHCP: " + packet.IsActualDhcp);
                        Console.WriteLine("MAC: " + packet.GetClientMac());
                        Console.WriteLine("Hostname: " + packet.GetHostName());
                        //loggingEvent += Encoding.ASCII.GetString(receivedResults.Buffer);
                        //Console.WriteLine(loggingEvent);
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }
    }
}