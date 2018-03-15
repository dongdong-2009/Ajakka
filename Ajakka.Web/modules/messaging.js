var amqp = require('amqplib/callback_api');
var configuration = require('../config/ajakkaConfiguration');


function generateUuid() {
    return Math.random().toString() +
        Math.random().toString() +
        Math.random().toString();
}

function SendMessageToQueue(response, message, rpcQueue){
    amqp.connect(configuration.messageQueueHostAddress, function(err, conn) {
        conn.createChannel(function(err, ch) {
            ch.assertQueue('', {exclusive: true}, function(err, q) {
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