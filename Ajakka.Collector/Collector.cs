using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Ajakka.Messaging;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Ajakka.Collector
{
    public class Collector:IDisposable
    {
        ICollectorDAL dal;
        ICollectorConfiguration collectorConfiguration;
        IConnection connection;
        IModel channel;

        private Collector(){

        }

        public Collector(ICollectorDAL dal, ICollectorConfiguration collectorConfiguration){
            if(dal == null){
                throw new ArgumentNullException("dal");
            }
            this.dal = dal;

            if(collectorConfiguration == null){
                throw new ArgumentNullException("collectorConfiguration");
            }
            this.collectorConfiguration = collectorConfiguration;
        }

        public void Listen(){
            var factory = new ConnectionFactory() { HostName = collectorConfiguration.MessageQueueHost };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            
            channel.ExchangeDeclare(exchange: collectorConfiguration.MessageQueueExchangeName, type: "fanout");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                            exchange: collectorConfiguration.MessageQueueExchangeName,
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
                var deviceDescriptor = ParseMesage(body);
                StoreDeviceInfo(deviceDescriptor);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not parse the message: " + ex);
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" Message:{0}{1}", Environment.NewLine, message);
            }
        }
        static DeviceDescriptorMessage ParseMesage(byte[] message){
            using(var stream = new MemoryStream(message)){
                var serializer = new DataContractJsonSerializer(typeof(DeviceDescriptorMessage));
                return (DeviceDescriptorMessage)serializer.ReadObject(stream);
            }
        }

        void StoreDeviceInfo(DeviceDescriptorMessage deviceInfo){
            dal.StoreDhcpEndpoint(deviceInfo.DeviceMacAddress, 
            deviceInfo.DeviceIpAddress, 
            deviceInfo.DeviceName, 
            deviceInfo.TimeStamp);
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
