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
    SendMessageToQueue(res, '{"FunctionName": "GetLatest", "PageNumber": "'+pageNumber+'", "PageSize": "'+pageSize+'"}');
});

router.get('/pageCount', function (req, res) {
    var pageSize = req.query.pageSize;
    if(!pageSize){
        pageSize = 10;
    }
    SendMessageToQueue(res, '{"FunctionName": "GetDhcpEndpointPageCount", "PageSize": "'+pageSize+'"}');
    
});

function SendMessageToQueue(response, message){
    amqp.connect(configuration.messageQueueHostAddress, function(err, conn) {
        conn.createChannel(function(err, ch) {
            ch.assertQueue('', {exclusive: true}, function(err, q) {
            var corr = generateUuid();
            ch.consume(q.queue, function(msg) {
                if (msg.properties.correlationId == corr) {
                    var responseObject = JSON.parse(msg.content.toString());
                    console.log(responseObject);
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
           
            ch.sendToQueue(configuration.collectorRpcQueue,
            new Buffer(message),
            { correlationId: corr, replyTo: q.queue });
            });
        });
        });
}

module.exports = router;