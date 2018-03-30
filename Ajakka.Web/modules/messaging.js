var amqp = require('amqplib/callback_api');
var configuration = require('../config/ajakkaConfiguration');


function generateUuid() {
    return Math.random().toString() +
        Math.random().toString() +
        Math.random().toString();
}

function SendMessageToQueue(response, message, rpcQueue){
    amqp.connect(configuration.messageQueueHostAddress, function(err, conn) {
        if(err){
            console.log('Failed to connect to the message queue host.');
            console.log(err);
            response.status(500).send(err);
            return;
        }
        conn.createChannel(function(err, ch) {
            if(err){
                console.log('Failed to create the channel.');
                console.log(err);
                response.status(500).send(err);
                return;
            }
            ch.assertQueue('', {exclusive: true}, function(err, q) {
                if(err){
                    console.log('Failed to connect to the message queue host.');
                    console.log(err);
                    response.status(500).send(err);
                    return;
                }
                var corr = generateUuid();
                ch.consume(q.queue, function(msg) {
                    if (msg.properties.correlationId == corr) {
                        var responseObject = JSON.parse(msg.content.toString());
                        if(responseObject.Error){
                            console.log("Received error: " + responseObject.Message);
                            response.status(500).send(responseObject);
                        }
                        else{
                            response.status(200).send(responseObject);
                        }
                        
                    setTimeout(function() { conn.close(); }, 500);
                    }
                }, {noAck: true});
            
                ch.sendToQueue(rpcQueue,
                new Buffer(message),
                { correlationId: corr, replyTo: q.queue });
            });
        });
    });
}

module.exports.SendMessageToQueue = SendMessageToQueue;