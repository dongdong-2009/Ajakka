var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var amqp = require('amqplib/callback_api');
var configuration = require('../config/ajakkaConfiguration');
var messaging = require('../modules/messaging');

router.post('/', function (req, res) {
    var pageNumber = req.body.pageNumber;
    var pageSize = req.body.pageSize;
    if(!pageSize){
        pageSize = 10;
    }
    if(!pageNumber){
        pageNumber = 0;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetLatest", "PageNumber": "'+pageNumber+'", "PageSize": "'+pageSize+'"}', configuration.collectorRpcQueue);
});

router.get('/pageCount', function (req, res) {
    var pageSize = req.query.pageSize;
    if(!pageSize){
        pageSize = 10;
    }
    messaging.SendMessageToQueue(res, '{"FunctionName": "GetDhcpEndpointPageCount", "PageSize": "'+pageSize+'"}', configuration.collectorRpcQueue);
    
});


module.exports = router;