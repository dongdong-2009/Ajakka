using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Ajakka.Blacklist{

    public class BlacklistConfiguration:IBlacklistConfiguration{
        
        IConfiguration configuration;
        
        public BlacklistConfiguration(){
            var jsonSource = new JsonConfigurationSource();
            jsonSource.Path = "blacklistconfig.json";
            
            var builder = new ConfigurationBuilder();
            builder.Add(jsonSource);

            configuration = builder.Build();
        }

        public string MessageQueueHost{
            get{return configuration["messageQueueHost"];}
        }

        public string MessageQueueExchangeName{
            get{return configuration["messageQueueExchangeName"];}
        }
       
        public string CommandProcessorRpcQueueName{
            get{return configuration["commandProcessorRpcQueueName"];}
        }

        public string AlertingEventQueueName{
            get{return configuration["alertingEventQueueName"];}
        }

        public string MessageQueueUserName{
            get{return Environment.GetEnvironmentVariable("AjakkaMqUser");}
        }

        public string MessageQueuePassword{
            get{return Environment.GetEnvironmentVariable("AjakkaMqPassword");}
        }

    }
}