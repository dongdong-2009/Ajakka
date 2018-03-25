function loadEndpoints(){
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }

    $.ajax({
        type: 'POST',
        url: '/api/endpoints',
        data:{pageNumber:currentPage, pageSize:10},
        dataType: 'json',
        success: fillTableWithEndpoints,
        error:showError
      });
    $.get({
        url:'/api/endpoints/pageCount?pageSize=10',
        success:showPageCount
    })
} 

function showPageCount(pageCountResponse){
    var pageCount = pageCountResponse.Content;
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }
    $('#pageCount').empty();
    if(currentPage > 0){
        var previousPage = currentPage - 1;
        $('#pageCount').append('<a href="/index?page='+previousPage+'"><i class="fas fa-caret-left"> ');    
    }
    currentPage++;
    $('#pageCount').append(' ' + currentPage + '/' + pageCount);

    if(currentPage < pageCount){
        var nextPage = currentPage;
        $('#pageCount').append('<a href="/index?page='+nextPage+'"> <i class="fas fa-caret-right"> ');    
    }
}

function showError(error){
    console.log(error);
    $('#endpointListContainer').empty();
    $('#endpointListContainer').append('<tr class="table-danger"><td colspan="4">Request error: '+error.statusText+'</td></tr>');
}

function getVendorShortName(vendorName){
    vendorName = vendorName.toLowerCase();
    let index = vendorName.indexOf('(');
    
    if(index > 0){
        vendorName = vendorName.substring(0,index);
    }
    vendorName = vendorName.replace(',','');
    vendorName = vendorName.replace(/\./g,'');    
    vendorName = vendorName.replace(/\ /g,'');
    vendorName = removeVendorNameSuffix(vendorName,'coltd');

    vendorName = removeVendorNameSuffix(vendorName,'co');

    vendorName = removeVendorNameSuffix(vendorName,'gmbh');

    vendorName = removeVendorNameSuffix(vendorName,'inc');
    return vendorName;
}

function removeVendorNameSuffix(vendorName, suffix){
    if(vendorName.endsWith(suffix)){
        vendorName = vendorName.substring(0,vendorName.length - suffix.length);
    }
    return vendorName;
}

function fillTableWithEndpoints(endpointsResponse){
    var endpoints = endpointsResponse.Content;
    $('#endpointListContainer').empty();
    
    endpoints.forEach(function(endpoint){
        var timestampUtc = moment.utc(endpoint.TimeStamp);
        var src = '/api/vendor/' + getVendorShortName(endpoint.VendorName);
        var timestamp = timestampUtc.local().format('YYYY/MM/DD, h:mm:ss A');
        var row = '<tr>';
        row += '<td>'+ formatMac(endpoint.DeviceMacAddress) + '<img alt="'+endpoint.VendorName+'" src="'+ src+'" class="vendor-logo"/> </td>';
        row += '<td>' + endpoint.DeviceIpAddress + '</td>';
        row += '<td>' + endpoint.DeviceName + '</td>';
        row += '<td colspan="2">' + timestamp + '</td>';
        row += '</tr>';
        $('#endpointListContainer').append(row);
        
    });
    setTimeout(loadEndpoints, 300000);
}

setTimeout(loadEndpoints, 100);

