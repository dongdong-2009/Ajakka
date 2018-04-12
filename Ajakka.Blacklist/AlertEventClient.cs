using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Blacklist{
    class AlertEventClient:IDisposable{
        IBlacklistConfiguration configuration;
        IConnection connection;
        IModel channel;
        IBasicProperties properties;

        public AlertEventClient(IBlacklistConfiguration config)
        {
            configuration = config;
            var factory = new ConnectionFactory() { HostName = configuration.MessageQueueHost, UserName = configuration.MessageQueueUserName, Password = configuration.MessageQueuePassword };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: config.AlertingEventQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            properties = channel.CreateBasicProperties();
            properties.Persistent = true;
        }

        public void SendMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            
            channel.BasicPublish(exchange: "",
                                 routingKey: configuration.AlertingEventQueueName,
                                 basicProperties: properties,
                                 body: messageBytes);
        }

        public void Close()
        {
            connection.Close();
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

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}