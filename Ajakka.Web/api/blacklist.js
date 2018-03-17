var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var amqp = require('amqplib/callback_api');
var configuration = require('../config/ajakkaConfiguration');
var messaging = require('../modules/messaging');

  
//  default 
router.get('/', function (req, res) {
    pageNumber = 0;
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetRules", "PageNumber": "'+pageNumber+'"}', configuration.blacklistRpcQueue);
});

//  /api/blacklist/1
router.get('/page/:pageNumber', function (req, res) {
    var pageNumber = req.params.pageNumber;
    if(!pageNumber){
        pageNumber = 0;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetRules", "PageNumber": "'+pageNumber+'"}', configuration.blacklistRpcQueue);
});

router.get('/pageCount', function (req, res) {
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetPageCount"}', configuration.blacklistRpcQueue);
    
});

//  /api/blacklist/rule/2
router.get('/rule/:id', function (req, res) {
    var id = req.params.id;
    if(!id){
        res.status(500).send( {Message:"No id specified"});
        return;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetRule", "RuleId": "'+id+'"}', configuration.blacklistRpcQueue);
});

// /api/blacklist/rule/2
router.delete('/rule/:id', function(req, res){
    var id = req.params.id;
    if(!id){
        res.status(500).send({Message:'No id specified'});
        return;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName":"DeleteRule","RuleId":"'+id+'"}', configuration.blacklistRpcQueue);
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
    messaging.SendMessageToQueue(res, '{"FunctionName": "AddRule", "RuleName": "'+name+'", "RulePattern":"'+pattern+'"}', configuration.blacklistRpcQueue);
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

    messaging.SendMessageToQueue(res, '{"FunctionName":"LinkAction","RuleId":"'+ruleId+'","ActionId":'+actionId+'}', configuration.blacklistRpcQueue);
});

module.exports = router;