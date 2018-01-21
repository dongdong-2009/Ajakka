using System;

namespace Ajakka.Sensor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Sensor started");
            var sensor = new DhcpSensor();
            Console.ReadLine();
            sensor.Stop();
        }
    }
}
