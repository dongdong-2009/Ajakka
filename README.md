# Ajakka
Monitors local network for DHCP packets and detects new devices connecting in.

## Requirements
Message broker requires Rabbit MQ: https://www.rabbitmq.com/install-standalone-mac.html

## Ajakka.Sensor
Standalone DHCP (IPv4) Sensor, sends messages to RabbitMQ when a DHCP packet is detected.

`sudo dotnet run `

Exchange: ajakkaExchange

## Ajakka.Collector
Collects messages from ajakkaExchange and stores them in a database (TBD).

`sudo dotnet run `
