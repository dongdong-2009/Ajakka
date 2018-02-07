var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var amqp = require('amqplib/callback_api');

function generateUuid() {
    return Math.random().toString() +
           Math.random().toString() +
           Math.random().toString();
  }

// 
router.get('/', function (req, res) {

    var args = process.argv.slice(2);

    amqp.connect('amqp://localhost', function(err, conn) {
    conn.createChannel(function(err, ch) {
        ch.assertQueue('', {exclusive: true}, function(err, q) {
        var corr = generateUuid();
        var num = parseInt(args[0]);

        console.log(' Requesting list');

        ch.consume(q.queue, function(msg) {
            console.log('got some message');
            if (msg.properties.correlationId == corr) {
                res.status(200).send(msg.content.toString());
           
            setTimeout(function() { conn.close(); }, 500);
            }
        }, {noAck: true});

        ch.sendToQueue('collector_dal_rpc_queue',
        new Buffer('{"FunctionName": "GetLatest", "PageNumber": "0", "PageSize": "0"}'),
        { correlationId: corr, replyTo: q.queue });
        });
    });
    });



  
});

module.exports = router;