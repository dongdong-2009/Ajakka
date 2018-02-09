var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var amqp = require('amqplib/callback_api');
var configuration = require('../config/ajakkaConfiguration');

function generateUuid() {
    return Math.random().toString() +
           Math.random().toString() +
           Math.random().toString();
  }


router.post('/', function (req, res) {
    var pageNumber = req.body.pageNumber;
    var pageSize = req.body.pageSize;
    if(!pageSize){
        pageSize = 10;
    }
    if(!pageNumber){
        pageNumber = 0;
    }
    amqp.connect(configuration.messageQueueHostAddress, function(err, conn) {
    conn.createChannel(function(err, ch) {
        ch.assertQueue('', {exclusive: true}, function(err, q) {
        var corr = generateUuid();
        ch.consume(q.queue, function(msg) {
           
            if (msg.properties.correlationId == corr) {
                res.status(200).send(msg.content.toString());
           
            setTimeout(function() { conn.close(); }, 500);
            }
        }, {noAck: true});
       
        ch.sendToQueue(configuration.collectorRpcQueue,
        new Buffer('{"FunctionName": "GetLatest", "PageNumber": "'+pageNumber+'", "PageSize": "'+pageSize+'"}'),
        { correlationId: corr, replyTo: q.queue });
        });
    });
    });



  
});

module.exports = router;