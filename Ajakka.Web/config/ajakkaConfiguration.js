function getMySqlUrl(){
    if(process.env.NODE_ENV == 'unittests'){
        return process.env.AjakkaWebTestMySql;
    }
    return process.env.AjakkaWebMySql;
}

module.exports.messageQueueHostAddress= 'amqp://localhost';
module.exports.collectorRpcQueue = 'collector_dal_rpc_queue';
module.exports.getMySqlUrl = getMySqlUrl;
module.exports.blacklistRpcQueue = 'blacklist_rpc_queue';