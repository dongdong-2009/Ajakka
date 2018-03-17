var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var configuration = require('../config/ajakkaConfiguration');
var messaging = require('../modules/messaging');


router.get('/pageCount', function (req, res) {
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetPageCount"}', configuration.alertingRpcQueue);
    
});

router.get('/actionTypes', function (req, res) {
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetActionTypes"}', configuration.alertingRpcQueue);
    
});

router.get('/:id', function (req, res) {
    var id = req.params.id;
    if(!id){
        res.status(500).send( {Message:"No id specified"});
        return;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetAction","ActionId":'+id+'}', configuration.alertingRpcQueue);
    
});

router.post('/exec/:id', function (req, res) {
    var id = req.params.id;
    if(!id){
        res.status(500).send( {Message:"No id specified"});
        return;
    }
    var message = req.body.alertMessage;
    if(!message){
        message = '';
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "Execute","ActionId":'+id+',"AlertMessage":"'+message+'"}', configuration.alertingRpcQueue);
    
});

//add action: name, configuration, type
router.post('/', function (req, res) {
    var name = req.body.name;
    if(!name){
        res.status(500).send( {Message:'"name" is required'});
        return;
    }
    var actionConfig = req.body.configuration;
    if(!actionConfig){
        actionConfig = '';
    }
    console.log('Action Configuration: ' + actionConfig);
    var actionType = req.body.type;
    if(!actionType){
        res.status(500).send( {Message:'"type" is required'});
        return;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "AddAction","ActionName":"'+name+'","ActionConfiguration":"'+actionConfig+'","ActionType":"'+actionType+'"}', configuration.alertingRpcQueue);
    
});


module.exports = router;