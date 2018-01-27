using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Ajakka.Sensor{

    class SensorConfiguration{
        
        IConfiguration configuration;
        
        public SensorConfiguration(){
            var jsonSource = new JsonConfigurationSource();
            jsonSource.Path = "sensorconfig.json";
            
            var builder = new ConfigurationBuilder();
            builder.Add(jsonSource);

            configuration = builder.Build();
        }

        public string InterfaceId{
            get{return configuration["interfaceId"];}
        }

        public string QueueName{
            get{return configuration["queueName"];}
        }

        public string IpAddress{
            get{return configuration["ipAddress"];}
        }

        public string MessageQueueHost{
            get{return configuration["messageQueueHost"];}
        }

        public string MessageQueueExchangeName{
            get{return configuration["messageQueueExchangeName"];}
        }
        public string MessageQueueRoutingKey{
            get{return configuration["messageQueueRoutingKey"];}
        }
        public bool EnableMessaging{
            get{return configuration["enableMessaging"] == "true";}
        }
    }
}