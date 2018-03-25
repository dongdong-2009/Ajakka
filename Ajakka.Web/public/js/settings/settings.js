function loadSettingsValues(){
    $('#errorMessage').hide();
    $('#successMessage').hide();
    $.get({
        url:'api/users/settings/showVendorLogos',
        success:function(result){
            var value = result.content;
            $('#showLogos').val(value);
        },
        error:function(err){
            alert('Failed to load settings from database.');
            console.log(err);
        }
    })
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
}

setTimeout(loadSettingsValues,100);