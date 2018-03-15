using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Blacklist{
    class RpcClient:IDisposable{
        IConnection connection;
        IModel channel;
        string replyQueueName;
        EventingBasicConsumer consumer;
        IBasicProperties properties;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        IBlacklistConfiguration configuration;
        
        public RpcClient(IBlacklistConfiguration config)
        {
            configuration = config;
            var factory = new ConnectionFactory() { HostName = configuration.MessageQueueHost };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            properties = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string SendMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: configuration.AlertingRpcQueueName,
                basicProperties: properties,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take(); ;
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
                    connection.Close();
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