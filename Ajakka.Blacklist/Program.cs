using System;

namespace Ajakka.Blacklist
{
    class Program
    {
        static void Main(string[] args)
        {
            IBlacklist blacklist = BlacklistFactory.CreateBlacklist();
            var config = new BlacklistConfiguration();

            Console.WriteLine("MessageQueueExchangeName: " + config.MessageQueueExchangeName);
            Console.WriteLine("MessageQueueHost: " + config.MessageQueueHost);
            Console.WriteLine("DALServerRpcQueueName: " + config.CommandProcessorRpcQueueName);
            
            using(var cmdProcessor = new CommandProcessor(config, blacklist)){
                cmdProcessor.Start();
                Console.WriteLine("CommandProcessor started");

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
