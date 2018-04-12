using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Ajakka.TestSend{
      class Configuration{
        
        IConfiguration configuration;
        
        public Configuration(){
            var jsonSource = new JsonConfigurationSource();
            jsonSource.Path = "config.json";
            
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

        
        public string MessageQueueUserName{
            get{return Environment.GetEnvironmentVariable("AjakkaMqUser");}
        }

        public string MessageQueuePassword{
            get{return Environment.GetEnvironmentVariable("AjakkaMqPassword");}
        }


    }
}