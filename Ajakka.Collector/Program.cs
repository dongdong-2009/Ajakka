using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ajakka.Collector{
    public class Program{

        static void Main(string[] args)
        {
            var config = new CollectorConfiguration();
            var collector = InitializeCollector();
            if(collector == null){
                return;
            }
            var factory = new ConnectionFactory() { HostName = config.MessageQueueHost };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: config.MessageQueueExchangeName, type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                exchange: config.MessageQueueExchangeName,
                                routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    collector.ProcessMessage(ea.Body);
                };
                channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static Collector InitializeCollector(){
            var connString = Environment.GetEnvironmentVariable("AjakkaConnection");
            if(string.IsNullOrEmpty(connString)){
                Console.WriteLine("AjakkaConnection environment variable is not set.");
                return null;
            }

            
            return new Collector(new DAL(connString));
        }
    }
}