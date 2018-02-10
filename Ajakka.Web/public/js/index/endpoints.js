function loadEndpoints(){

    $.ajax({
        type: 'POST',
        url: './api/endpoints',
        data:{pageNumber:0, pageSize:10},
        dataType: 'json',
        success: fillTableWithEndpoints,
        error:showError
      });
    $.get({
        url:'./api/endpoints/pageCount?pageSize=10',
        success:showPageCount
    })
} 

function showPageCount(pageCount){
    var currentPage = 1;
    $('#pageCount').append('page ' + currentPage + '/' + pageCount);
}

function showError(error){
    console.log(error);
    $('#endpointListContainer').empty();
    $('#endpointListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.statusText+'</td></tr>');
}

function fillTableWithEndpoints(endpoints){
    $('#endpointListContainer').empty();
    
    endpoints.forEach(function(endpoint){
        var timestampUtc = moment.utc(endpoint.TimeStamp);
        
        var timestamp = timestampUtc.local().format('YYYY/MM/DD, h:mm:ss A');
        var row = '<tr>';
        row += '<td>' + formatMac(endpoint.DeviceMacAddress) + '</td>';
        row += '<td>' + endpoint.DeviceIpAddress + '</td>';
        row += '<td>' + endpoint.DeviceName + '</td>';
        row += '<td>' + timestamp + '</td>';
        row += '</tr>';
        $('#endpointListContainer').append(row);
        
    });
}

setTimeout(loadEndpoints, 1000);

