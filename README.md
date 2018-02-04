# Ajakka
Monitors local network for DHCP packets and detects new devices connecting in.

## Requirements
Services communicate over Rabbit MQ: https://www.rabbitmq.com/install-standalone-mac.html

## Ajakka.Sensor
Standalone DHCP (IPv4) Sensor, sends messages to RabbitMQ when a DHCP packet is detected.
~~~~
sudo dotnet run 
~~~~

Configured by sensorconfig.json:
~~~~
{
    "enableMessaging":"true",
    "queueName":"ajakka.sensor",
    "messageQueueHost":"localhost",,
    "messageQueueExchangeName":"ajakkaExchange"
}
~~~~

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