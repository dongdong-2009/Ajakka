using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Collector{
    public class Program{

        static void Main(string[] args)
        {
            var collector = InitializeCollector();
            if(collector == null){
                return;
            }
            collector.Listen();
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        static Collector InitializeCollector(){
            var connString = Environment.GetEnvironmentVariable("AjakkaConnection");
            if(string.IsNullOrEmpty(connString)){
                Console.WriteLine("AjakkaConnection environment variable is not set.");
                return null;
            }
            var config = new CollectorConfiguration();
            return new Collector(new DAL(connString), config);
        }
    }
}