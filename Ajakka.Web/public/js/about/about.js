function testConnection(url, statusDivId ){
    $.get({
        url:url,
        success:function(){
            let statusElement = $(statusDivId);
            statusElement.html('OK');
            statusElement.addClass('text-success')
        },
        error:function(error){
            let statusElement = $(statusDivId);
            statusElement.html('ERROR');
            statusElement.addClass('text-danger')
            console.log(error);
        }
    })
}


setTimeout(function(){
    testConnection('./api/endpoints/pageCount?pageSize=10', "#collectorStatus");
    testConnection('./api/users', "#dbStatus");
    testConnection('./api/blacklist/', "#blacklistStatus");
}, 500);
