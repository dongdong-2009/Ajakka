function loadSettingsValues(){
    $('#errorMessage').hide();
    $('#successMessage').hide();
    $.get({
        url:'api/users/settings/showVendorLogos',
        success:function(result){
            var value = result.content;
            if(value){
                $('#showLogos').val(value);
            }
            else{
                $('#showLogos').val(0);
            }
        },
        error:function(err){
            alert('Failed to load settings from database.');
            console.log(err);
        }
    });
    $.get({
        url:'api/users/settings/hideSensorColumn',
        success:function(result){
            var value = result.content;
            if(value){
                if(value == "1"){
                    $('#showSensorNames').val('no');
                }
                else{
                    $('#showSensorNames').val('yes');
                }
                
            }
            else{
                $('#showSensorNames').val('yes');
            }
        },
        error:function(err){
            alert('Failed to load settings from database.');
            console.log(err);
        }
    });
}

function saveAllSettings(){
    $('#errorMessage').hide();
    $('#successMessage').hide();
    let value = $('#showLogos').val();
    $.ajax({
        method:'put',
        url:'api/users/settings/showVendorLogos/'+value,
        success:function(){
            $('#successMessage').show();
        },
        error:function(error){
            console.log(error);
            $('#errorMessage').show();
        }
    });
    let hideSensors = $('#showSensorNames').val();
    if(hideSensors == 'yes'){
        hideSensors = '0';
    }
    else{
        hideSensors = '1';
    }
    $.ajax({
        method:'put',
        url:'api/users/settings/hideSensorColumn/'+hideSensors,
        success:function(){
            $('#successMessage').show();
        },
        error:function(error){
            console.log(error);
            $('#errorMessage').show();
        }
    });
    
    window.localStorage.vendorLogos = value;
    window.localStorage.hideSensorColumn = hideSensors;
}

setTimeout(loadSettingsValues,100);