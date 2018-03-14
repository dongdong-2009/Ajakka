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

  
//  default 
router.get('/', function (req, res) {
    SendMessageToQueue(res, '{"FunctionName": "GetRules", "PageNumber": 0}');
});

//  /api/blacklist/1
router.get('/page/:pageNumber', function (req, res) {
    var pageNumber = req.params.pageNumber;
    if(!pageNumber){
        pageNumber = 0;
    }
    SendMessageToQueue(res, '{"FunctionName": "GetRules", "PageNumber": "'+pageNumber+'"}');
});

router.get('/pageCount', function (req, res) {
    SendMessageToQueue(res, '{"FunctionName": "GetPageCount"}');
    
});

//  /api/blacklist/rule/2
router.get('/rule/:id', function (req, res) {
    var id = req.params.id;
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

//links action id to rule
router.put('/linkaction/:ruleId/:actionId', function (req, res) {
    var ruleId = req.params.ruleId;
    var actionId = req.params.actionId;

    if(!ruleId){
        res.status(500).send( {Message:"ruleId cannot be empty"});
        return;
    }
    if(!actionId){
        res.status(500).send( {Message:"actionId cannot be empty"});
        return;
    }

    SendMessageToQueue(res, '{"FunctionName":"LinkAction","RuleId":"'+ruleId+'","ActionId":'+actionId+'}');
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