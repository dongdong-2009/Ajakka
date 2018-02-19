using System;
using Ajakka.Collector;

namespace Ajakka.CollectorMock
{
    class Program
    {
        static void Main(string[] args)
        {
            ICollectorDAL dal = new Ajakka.CollectorMock.DAL();
            var config = new CollectorConfiguration();

            Console.WriteLine("MessageQueueExchangeName: " + config.MessageQueueExchangeName);
            Console.WriteLine("MessageQueueHost: " + config.MessageQueueHost);
            Console.WriteLine("DALServerRpcQueueName: " + config.DALServerRpcQueueName);
            
            using(var dalServer = new DALServerMock(config, dal)){
                dalServer.Start();
                Console.WriteLine("DALServer started");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            
        }
    }
}
