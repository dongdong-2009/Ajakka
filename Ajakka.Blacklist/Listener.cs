using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;

namespace Ajakka.Blacklist
{
    public class Listener:IDisposable
    {
        IBlacklist dal;
        IBlacklistConfiguration blacklistConfiguration;
        IConnection connection;
        IModel channel;

        private Listener(){

        }

        public Listener(IBlacklist dal, IBlacklistConfiguration blacklistConfiguration){
            if(dal == null){
                throw new ArgumentNullException("dal");
            }
            this.dal = dal;

            if(blacklistConfiguration == null){
                throw new ArgumentNullException("blacklistConfiguration");
            }
            this.blacklistConfiguration = blacklistConfiguration;
        }

        public void Listen(){
            var factory = new ConnectionFactory() { HostName = blacklistConfiguration.MessageQueueHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            
            channel.ExchangeDeclare(exchange: blacklistConfiguration.MessageQueueExchangeName, type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                            exchange: blacklistConfiguration.MessageQueueExchangeName,
                            routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                ProcessMessage(ea.Body);
            };
            channel.BasicConsume(queue: queueName,
                                autoAck: true,
                                consumer: consumer);
        
        }

        private void ProcessMessage(byte[] body)
        {
            try{
                var message = Encoding.UTF8.GetString(body);
                var deviceDescriptor = ParseMesage(message);
                ProcessDevice(deviceDescriptor);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not parse the message: " + ex);
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" Message:{0}{1}", Environment.NewLine, message);
            }
        }

        private void ProcessDevice(dynamic deviceDescriptor)
        {
            Console.WriteLine("processing device " + deviceDescriptor.DeviceMacAddress);
            var pageCount = dal.GetRulePageCount();
            Rule match;
            for(var page = 0; page < pageCount; page++){
                if(ProcessPageWithRules(dal.GetRules(page), deviceDescriptor.DeviceMacAddress, deviceDescriptor.DeviceIpAddress, deviceDescriptor.DeviceName, out match)){
                    SendAlert(deviceDescriptor, match);
                    break;
                }
            }
        }

        private void SendAlert(dynamic device, Rule match){
            Console.WriteLine("Blacklist rule match: " + match.Name);
            Console.WriteLine("device: " + device);
            
        }

        private bool ProcessPageWithRules(Rule[] rules, string mac, string ip, string hostname, out Rule match){
            foreach(var rule in rules){
                if(rule.IsMatch(mac) || rule.IsMatch(ip) || rule.IsMatch(hostname)){
                    match = rule;
                    return true;
                }
            }
            match = null;
            return false;
        }

        static dynamic ParseMesage(string message){
           
            var definition = new {DeviceName = "", DeviceMacAddress = "", DeviceIpAddress = "", TimeStamp = DateTime.MinValue};
            return JsonConvert.DeserializeAnonymousType(message, definition);
        
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channel.Dispose();
                    connection.Dispose();

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
