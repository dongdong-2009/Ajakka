# Ajakka
Monitors local network for DHCP packets and detects new devices connecting in.

## Requirements
Services communicate over Rabbit MQ: https://www.rabbitmq.com/install-standalone-mac.html

## Ajakka.Sensor
Standalone DHCP (IPv4) Sensor, sends messages to RabbitMQ when a DHCP packet is detected. Requires elevated rights.
~~~~
sudo dotnet run 
~~~~

Configured by sensorconfig.json:
~~~~
{
    "enableMessaging":"true",
    "messageQueueHost":"localhost",,
    "messageQueueExchangeName":"ajakkaSensorExchange"
}
~~~~

*enableMessaging* - when set to "true", sensor sends information about new endpoints to message queue.

*messageQueueHost* - server hosting the message queue.

*messageQueueExchangeName* - name of the exchange the Collector is listening on for Sensor messages. Sensor sends messages about new endpoints to this exchange.

## Ajakka.Collector
Collects messages from the configured exchange and stores them in a database.

~~~~
dotnet run
~~~~

Requires MySql database connection string in AjakkaConnection environment variable

Additional configuration in collectorconfig.json:
~~~~
{
    "messageQueueExchangeName":"ajakkaSensorExchange",
    "messageQueueHost":"localhost",
    "dalServerRpcQueueName":"collector_dal_rpc_queue"
}
~~~~

*messageQueueHost* - server hosting the message queue.

*messageQueueExchangeName* - name of the exchange the Collector is listening on. Sensor sends messages about new endpoints to this exchange. This value needs to match the "messageQueueExchangeName" configuration for Ajakka.Sensor.

*dalServerRpcQueueName* - name of the queue the Collector uses for listening to RPC requests for information stored in the Collector database.

## Ajakka.Collector.Tests
Requires MySql database connection string in AjakkaTestConnection environment variable

~~~~
dotnet xunit
~~~~

## Ajakka.DbInit
Creates required tables in the target mysql database. The database has to exist and the connection string needs to contain its name.
~~~~
dotnet run [environment variable storing connection string]
~~~~

## Ajakka.Web
Website

Connection configuration is in /config/ajakkaConfiguration.js:

~~~~
module.exports.messageQueueHostAddress= 'amqp://localhost';
module.exports.collectorRpcQueue = 'collector_dal_rpc_queue';
~~~~

*messageQueueHostAddress* - server hosting the message queue.

*collectorRpcQueue* - name of the queue the Collector uses for listening to RPC requests for information stored in the Collector database. This should match the value of dalServerRpcQueueName in Ajakka.Collector's configuration file.

Start as:
~~~~
npm run start
~~~~
