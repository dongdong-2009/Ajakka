var rabbitMqHostAddress= 'localhost/';

function getMySqlUrl(){
    if(process.env.NODE_ENV == 'unittests'){
        return process.env.AjakkaWebTestMySql;
    }
    return process.env.AjakkaWebMySql;
}

function getRabbitMqUserName(){
    if(process.env.AjakkaMqUser)
        return process.env.AjakkaMqUser;
    
    return 'guest';
}

function getRabbitMqPassword(){
    if(process.env.AjakkaMqPassword)
        return process.env.AjakkaMqPassword;
    
    return 'guest';
}

function getMessageQueueHostAddress(){
    var addr = 'amqp://'+getRabbitMqUserName()+':'+getRabbitMqPassword() +'@'+rabbitMqHostAddress;
    return addr;
}

module.exports.getMessageQueueHostAddress= getMessageQueueHostAddress; 
module.exports.collectorRpcQueue = 'collector_dal_rpc_queue';
module.exports.getMySqlUrl = getMySqlUrl;
module.exports.blacklistRpcQueue = 'blacklist_rpc_queue';
module.exports.alertingRpcQueue = 'alerting_rpc_queue';
module.exports.getMqUser = getRabbitMqUserName;
module.exports.getMqPassword = getRabbitMqPassword;