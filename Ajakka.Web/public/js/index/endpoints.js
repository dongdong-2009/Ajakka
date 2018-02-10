function loadEndpoints(){
    var currentPage = $('#currentPage').text();
    console.log(currentPage);
    if(!currentPage){
        currentPage = 0;
    }

    $.ajax({
        type: 'POST',
        url: './api/endpoints',
        data:{pageNumber:currentPage, pageSize:10},
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
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }
    
    if(currentPage > 0){
        var previousPage = currentPage - 1;
        $('#pageCount').append('<a href="./index?page='+previousPage+'">&lt;&lt; ');    
    }
    currentPage++;
    $('#pageCount').append(' ' + currentPage + '/' + pageCount);

    if(currentPage < pageCount){
        var nextPage = currentPage;
        $('#pageCount').append('<a href="./index?page='+nextPage+'"> &gt;&gt; ');    
    }
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

