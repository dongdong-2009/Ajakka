using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Ajakka.Sensor{
    class DhcpSensor{
        UdpClient client;
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
                    string loggingEvent = "";
                    Console.WriteLine("Starting to listen on " + udpClient.Client.LocalEndPoint);
                    while (!stop)
                    {
                        var receivedResults = await udpClient.ReceiveAsync();
                        loggingEvent += Encoding.ASCII.GetString(receivedResults.Buffer);
                        Console.WriteLine(loggingEvent);
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }
    }
}