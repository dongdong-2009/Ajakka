using System;

namespace Ajakka.Collector{
    public class Program{

        static void Main(string[] args)
        {
            var connString = Environment.GetEnvironmentVariable("AjakkaConnection");
            if(string.IsNullOrEmpty(connString)){
                Console.WriteLine("AjakkaConnection environment variable is not set.");
                return;
            }

            ICollectorDAL dal = new DAL(connString);
            var config = new CollectorConfiguration();
            var collector = new Collector(new DAL(connString), config);

            collector.Listen();
            Console.WriteLine("Collector started");

            var dalServer = new DALServer(config, dal);
            dalServer.Start();
            Console.WriteLine("DALServer started");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

    }
}