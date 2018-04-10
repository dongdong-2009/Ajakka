function loadSettingsValues(){
    $('#errorMessage').hide();
    $('#successMessage').hide();
    
    loadValue('showVendorLogos',function(val){
        if(val){
            $('#showLogos').val(val);
        }
        else{
            $('#showLogos').val(0);
        }
    });

    loadValue('hideSensorColumn', function(val){
        if(val == "1"){
            $('#showSensorNames').val('no');
        }
        else{
            $('#showSensorNames').val('yes');
        }
    });

    loadValue('endpointsPageSize', function(val){
        if(val)
        {
            $('#endpointsPageSize').val(val);
        }
        else{
            $('endpointsPageSize').val(10);
        }
    });
   
}

function loadValue(name, success){
    $.get({
        url:'api/users/settings/'+name,
        success:function(result){
            var value = result.content;
            success(value);
            
        },
        error:function(err){
            alert('Failed to load settings from database.');
            console.log(err);
        }
    });
}

function saveValue(name, value){
    $.ajax({
        method:'put',
        url:'api/users/settings/'+name+'/'+value,
        success:function(){
            $('#successMessage').show();
        },
        error:function(error){
            console.log(error);
            $('#errorMessage').show();
        }
    });
}

function saveAllSettings(){
    $('#errorMessage').hide();
    $('#successMessage').hide();
    let value = $('#showLogos').val();
    saveValue('showVendorLogos',value);

    let hideSensors = $('#showSensorNames').val();
    if(hideSensors == 'yes'){
        hideSensors = '0';
    }
    else{
        hideSensors = '1';
    }
    saveValue('hideSensorColumn',hideSensors);

    let endpointsPageSize=$('#endpointsPageSize').val();
    saveValue('endpointsPageSize',endpointsPageSize)
   
    window.localStorage.vendorLogos = value;
    window.localStorage.hideSensorColumn = hideSensors;
    window.localStorage.endpointsPageSize = endpointsPageSize;
}

setTimeout(loadSettingsValues,100);