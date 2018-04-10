function loadPage(){
    loadUserSettings().then(function(){
        loadEndpoints();
    }).catch(function(err){
        alert('Failed to load settings from database.');
        console.log(err);
    });
}

function loadEndpoints(){
    var currentPage = $('#currentPage').text();
    if(!currentPage){
        currentPage = 0;
    }

    $.ajax({
        type: 'POST',
        url: '/api/endpoints',
        data:{pageNumber:currentPage, pageSize:window.localStorage.endpointsPageSize},
        dataType: 'json',
        success: fillTableWithEndpoints,
        error:showError
      });
    $.get({
        url:'/api/endpoints/pageCount?pageSize='+window.localStorage.endpointsPageSize,
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
        if(window.localStorage.vendorLogos == 1){ //pictures only
            row += '<td>'+ formatMac(endpoint.DeviceMacAddress) + '<img alt="'+endpoint.VendorName+'" src="'+ src+'" class="vendor-logo"/> </td>';
        }
        else if(window.localStorage.vendorLogos == 2){ //pictures and names
            row += '<td><span class="device-mac">'+ formatMac(endpoint.DeviceMacAddress) + '</span> <span class="device-vendor-name">'+endpoint.VendorName+'</span><img alt="'+endpoint.VendorName+'" src="'+ src+'" class="vendor-logo"/> </td>';
        }
        else{ //names only
            row += '<td><span class="device-mac">'+ formatMac(endpoint.DeviceMacAddress) +'</span> <span class="device-vendor-name">'+endpoint.VendorName+'</span></td>';
        }
        
        row += '<td>' + endpoint.DeviceIpAddress + '</td>';
        row += '<td>' + endpoint.DeviceName + '</td>';

        row += '<td class="sensor-column">' + endpoint.DetectedBy + '</td>';
        row += '<td colspan="2">' + timestamp + '</td>';
        row += '</tr>';
        $('#endpointListContainer').append(row);
        
    });
    if(window.localStorage.hideSensorColumn == 1){
        $('.sensor-column').each(function(item){
            $(this).hide();
        });
    }
    setTimeout(loadEndpoints, 300000);
}

setTimeout(loadPage, 100);

