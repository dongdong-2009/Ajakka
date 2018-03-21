using System;
using System.IO;

namespace Ajakka.Blacklist
{
    class Program
    {
        const string BlacklistStorageFileName = "blacklist.json";

        static void Main(string[] args)
        {
            IBlacklist blacklist = BlacklistFactory.CreateBlacklist();
            if(File.Exists(BlacklistStorageFileName)){
                try{
                    ((IBlacklistStorage)blacklist).Load(BlacklistStorageFileName);
                }
                catch(Exception ex){
                    Console.WriteLine("Failed to load blacklist from file: " + ex);
                }
            }
            
            var config = new BlacklistConfiguration();

            Console.WriteLine("MessageQueueExchangeName: " + config.MessageQueueExchangeName);
            Console.WriteLine("MessageQueueHost: " + config.MessageQueueHost);
            Console.WriteLine("DALServerRpcQueueName: " + config.CommandProcessorRpcQueueName);
            try{
                using(var cmdProcessor = new CommandProcessor(config, blacklist)){
                    cmdProcessor.Start();
                    Console.WriteLine("CommandProcessor started");
                    using(var listener = new Listener(blacklist, config)){
                        listener.Listen();
                        Console.WriteLine("Listener started");
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine("Fatal error: " + ex);
            }
            finally{
                try{
                    ((IBlacklistStorage)blacklist).Save(BlacklistStorageFileName);
                }
                catch(Exception ex){
                    Console.WriteLine("Failed to save blacklist to file: " + ex);
                }
            } 
            
        }
    }
}
