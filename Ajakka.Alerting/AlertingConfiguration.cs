using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Ajakka.Alerting{

    public class AlertingConfiguration:IAlertingConfiguration{
        
        IConfiguration configuration;
        
        public AlertingConfiguration(){
            var jsonSource = new JsonConfigurationSource();
            jsonSource.Path = "alertingconfig.json";
            
            var builder = new ConfigurationBuilder();
            builder.Add(jsonSource);

            configuration = builder.Build();
        }

        public string MessageQueueHost{
            get{return configuration["messageQueueHost"];}
        }
       
        public string CommandProcessorRpcQueueName{
            get{return configuration["commandProcessorRpcQueueName"];}
        }
    }
}