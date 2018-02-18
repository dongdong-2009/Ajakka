function testCollectorConnection(){
    $.get({
        url:'./api/endpoints/pageCount?pageSize=10',
        success:showCollectorSuccess,
        error:showCollectorFailure
    })
}

function showCollectorSuccess(){
    let statusElement = $('#collectorStatus');
    statusElement.html('OK');
    statusElement.addClass('text-success')
}

function showCollectorFailure(error){
    let statusElement = $('#collectorStatus');
    statusElement.html('ERROR');
    statusElement.addClass('text-danger')
    console.log(error);
}

function testDbConnection(){
    $.get({
        url:'./api/users',
        success:showDbSuccess,
        error:showDbFailure
    })
}

function showDbSuccess(){
    let statusElement = $('#dbStatus');
    statusElement.html('OK');
    statusElement.addClass('text-success')
}

function showDbFailure(error){
    let statusElement = $('#dbStatus');
    statusElement.html('ERROR');
    statusElement.addClass('text-danger')
    console.log(error);
}

setTimeout(function(){
    testDbConnection();
    testCollectorConnection();
}, 500);
