using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ajakka.TestSend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var command = "";
            do{
                command = Console.ReadLine();
                if(command.StartsWith("s"))
                {
                    ProcessSendCommand(command);
                }
                if(command.StartsWith("b"))
                {
                    ProcessBroadcastCommand(command);
                }
            }while(command != "x");
        }


        private static void ProcessSendCommand(string command)
        {
            var data = command.Split(' ');
            IPAddress target;
            IPAddress.TryParse(data[1], out target);
            if(data.Length == 3 && target != null)
            {
                SendData(IPAddress.Parse(data[1]),data[2]);
            }
            else
            {
                Console.WriteLine("usage: send <ip address> <data>");
            }
        }

        private static void ProcessBroadcastCommand(string command)
        {
            var data = command.Split(' ');
            if(data.Length == 2)
            {
                BroadcastData(data[1]);
            }
            else
            {
                Console.WriteLine("usage: broadcast <data>");
            }
        }

        private static async void SendData(IPAddress ip, string data)
        {
            Console.WriteLine(string.Format("Sending data {0} to ip {1}", data, ip));
            UdpClient client = new UdpClient();
            var datagram = Encoding.UTF8.GetBytes(data);
            await client.SendAsync(datagram,datagram.Length,new IPEndPoint(ip,67));
        }

        private static async void BroadcastData(string data)
        {
            Console.WriteLine("Sending data (broadcast): " + data);
            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            var datagram = Encoding.UTF8.GetBytes(data);
            await client.SendAsync(datagram,datagram.Length,"255.255.255.255",67);
        }
    }
}
