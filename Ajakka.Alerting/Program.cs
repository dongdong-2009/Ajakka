using System;
using System.IO;

namespace Ajakka.Alerting
{
    class Program
    {
        const string AlertingStorageFileName = "alertactions.json";
        static void Main(string[] args)
        {

            var actionStore = ActionStoreFactory.GetActionStore();
            var config = new AlertingConfiguration();
            
            if(File.Exists(AlertingStorageFileName)){
                try{
                    ((IAlertingStorage)actionStore).Load(AlertingStorageFileName);
                }
                catch(Exception ex){
                    Console.WriteLine("Failed to load alert actions from file: " + ex);
                }
            }

            Console.WriteLine("MessageQueueHost: " + config.MessageQueueHost);
            Console.WriteLine("DALServerRpcQueueName: " + config.CommandProcessorRpcQueueName);
            try{
                using(var cmdProcessor = new CommandProcessor(config, actionStore)){
                    cmdProcessor.Start();
                    Console.WriteLine("CommandProcessor started");
                    using(var alertEventProcessor = new AlertEventProcessor(config, actionStore)){
                        Console.WriteLine("AlertEventProcessor started" + Environment.NewLine);

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
                    ((IAlertingStorage)actionStore).Save(AlertingStorageFileName);
                }
                catch(Exception ex){
                    Console.WriteLine("Failed to save alert actions to file: " + ex);
                }
            }
        }
    }
}
