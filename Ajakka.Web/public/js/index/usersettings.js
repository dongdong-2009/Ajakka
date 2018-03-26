function loadUserSettings(){
    return new Promise(function(resolve, reject){
        if(window.localStorage.vendorLogos){
            resolve();
            return;
        }
    
        $.get({
            url:'api/users/settings/showVendorLogos',
            success:function(result){
                window.localStorage.vendorLogos = result.content;
                resolve();
            },
            error:function(err){
                reject(err);
            }
        })
    });
    
}