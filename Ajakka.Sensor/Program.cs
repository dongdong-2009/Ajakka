using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Ajakka.Sensor
{
    public class Program
    {
        public static void Main()
        {
            var sensorConfiguration = new SensorConfiguration();
            DhcpSensor sensor = new DhcpSensor(sensorConfiguration);

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
            sensor.Stop();
        }


       
    }
}
