# Ajakka
Monitors local network for DHCP packets and detects new devices connecting in.

## Requirements
Services communicate over Rabbit MQ: https://www.rabbitmq.com/install-standalone-mac.html

## Setup
Install Rabbit MQ.
Install MySQL database.
Configure Ajakka.Sensor (see below) to provide the address of the Rabbit MQ server (unless on localhost).
Configure Ajakka.Collector to provide the address of the Rabbit MQ server (unless on localhost).
Set environment variable AjakkaConnection to contain connection string to MySql database (including database name). 

Example (Windows):
~~~~
server=127.0.0.1;uid=root;Password=mypassword;database=ajakka
~~~~
Example (Mac):
~~~~
server=127.0.0.1;uid=root;pwd=mypassword;database=ajakka
~~~~

Set environment variable AjakkaWebMySql to MySql URL:
~~~~
mysql://root:mypassword@127.0.0.1/ajakka
~~~~

Run Ajakka.Sensor:
~~~~
dotnet run
~~~~

Run Ajakka.Collector:
~~~~
dotnet run
~~~~

Install and run Ajakka.Web:
~~~~
npm install
npm run start
~~~~

## Projects in the solution
### Ajakka.Sensor
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

### Ajakka.Collector
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

### Ajakka.Collector.Tests
Requires MySql database connection string in AjakkaTestConnection environment variable

~~~~
dotnet xunit
~~~~

### Ajakka.DbInit
Creates required tables in the target mysql database. The database has to exist and the connection string needs to contain its name.
~~~~
dotnet run [environment variable storing connection string]
~~~~

### Ajakka.Alerting
Provides alerting functionality. Used by Ajakka.Blacklist, stores alert actions and their configuration.
~~~~
dotnet run
~~~~

### Ajakka.Alerting.Tests
Tests for Ajakka.Alerting project.
~~~~
dotnet xunit
~~~~


### Ajakka.Blacklist
Provides functionality for blacklist rules. Stores rules and their configuration. Evaluates devices detected by sensor and when the devices match a rule, a linked alert action is executed via Ajakka.Alerting.
~~~~
dotnet run
~~~~

### Ajakka.Blacklist.Tests
Tests for Ajakka.Blacklist project.
~~~~
dotnet run
~~~~


### Ajakka.Web
Website
MySql connection configuration is expected in environment variables:

~~~~
export AjakkaWebMySql="mysql://user:pass@host/db"
export AjakkaWebTestMySql="mysql://user:pass@host/db"
~~~~

Message queue connection configuration is in /config/ajakkaConfiguration.js:

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

#### vendor images
Vendor images are not included (copyright reasons). Add vendor logos to Ajakka.Web\images\vendor. 

Supported formats (extensions):jpg, png.

Naming convention: see function getVendorShortName() in /public/js/index/endpoint.js to get details on how image name is constructed from vendor name:

1) convert to lower caps

2) remove all within ()

3) remove commas

4) remove spaces

5) remove full stops

6) remove 'coltd' from the end of the string

7) remove 'co' from the end of the string

8) remove 'gmbh' from the end of the string

9) remove 'inc' from the end of the string

You can also check the *imagedownloader* project that searches for logos on Bing and downloads them to files following the same file naming convention.

### Imagedownloader
A simple program that gets vendor names from list, searches for their logos using Bing, and saves them as picture files to be used with Ajakka.Web.