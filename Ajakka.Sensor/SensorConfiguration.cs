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

        public string MessageQueueHost{
            get{return configuration["messageQueueHost"];}
        }

        public string MessageQueueExchangeName{
            get{return configuration["messageQueueExchangeName"];}
        }

        public bool EnableMessaging{
            get{return configuration["enableMessaging"] == "true";}
        }

        public string SensorName{
            get {return Environment.MachineName;}
        }
    }
}