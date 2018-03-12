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

//  /api/blacklist/?pageNumber=2
router.get('/', function (req, res) {
    var pageNumber = req.query.pageNumber;
    if(!pageNumber){
        pageNumber = 0;
    }
    SendMessageToQueue(res, '{"FunctionName": "GetRules", "PageNumber": "'+pageNumber+'"}');
});

router.get('/pageCount', function (req, res) {
    SendMessageToQueue(res, '{"FunctionName": "GetPageCount"}');
    
});

//  /api/blacklist/rule/?id=2
router.get('/rule', function (req, res) {
    var id = req.query.id;
    if(!id){
        res.status(500).send( {Message:"No id specified"});
        return;
    }
    SendMessageToQueue(res, '{"FunctionName": "GetRule", "RuleId": "'+id+'"}');
});

//creates a new rule
router.post('/', function (req, res) {
    var name = req.body.name;
    var pattern = req.body.pattern;
    if(!name){
        res.status(500).send( {Message:"Name cannot be empty"});
        return;
    }
    if(!pattern){
        pattern = "";
    }
    SendMessageToQueue(res, '{"FunctionName": "AddRule", "RuleName": "'+name+'", "RulePattern":"'+pattern+'"}');
});

function SendMessageToQueue(response, message){
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
           
            ch.sendToQueue(configuration.blacklistRpcQueue,
            new Buffer(message),
            { correlationId: corr, replyTo: q.queue });
            });
        });
        });
}

module.exports = router;