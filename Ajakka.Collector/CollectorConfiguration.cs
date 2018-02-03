using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Ajakka.Collector{

    class CollectorConfiguration:ICollectorConfiguration{
        
        IConfiguration configuration;
        
        public CollectorConfiguration(){
            var jsonSource = new JsonConfigurationSource();
            jsonSource.Path = "collectorconfig.json";
            
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
       
    }
}