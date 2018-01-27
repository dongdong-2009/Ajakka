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

        public string ChannelName{
            get{return configuration["channelName"];}
        }

        public string IpAddress{
            get{return configuration["ipAddress"];}
        }
    }
}