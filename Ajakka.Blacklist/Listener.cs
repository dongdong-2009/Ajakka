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

        AlertEventClient alertEventClient;

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
            alertEventClient = new AlertEventClient(blacklistConfiguration);
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
            for(var page = 0; page < pageCount; page++){
                ProcessPageWithRules(dal.GetRules(page), deviceDescriptor);
            }
        }

        private void SendAlert(dynamic device, Rule match){
            Console.WriteLine("Blacklist rule match: " + match.Name);
            Console.WriteLine("device: " + device);
            alertEventClient.SendMessage("{\"MessageType\":\"BlacklistMatch\",\"RuleId\":\""+match.Id+"\",\"Mac\":\""+ device.DeviceMacAddress+"\",\"Ip\":\""+device.DeviceIpAddress +"\",\"Name\":\""+device.DeviceName+"\"}");
        }

        private void ProcessPageWithRules(Rule[] rules, dynamic deviceDescriptor){
            foreach(var rule in rules){
                if(rule.IsMatch(deviceDescriptor.DeviceMacAddress) || rule.IsMatch(deviceDescriptor.DeviceIpAddress) || rule.IsMatch(deviceDescriptor.DeviceName)){
                    SendAlert(deviceDescriptor, rule);
                }
            }
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
                    alertEventClient.Dispose();
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
