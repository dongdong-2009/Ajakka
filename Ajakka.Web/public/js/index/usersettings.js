function loadUserSettings(){
    return new Promise(function(resolve, reject){
        if(window.localStorage.vendorLogos){
            resolve();
            return;
        }
    
        $.get({
            url:'api/users/settings/showVendorLogos',
            success:function(result){
                let val = result.content;
                if(!val){
                    val = 0;
                }
                window.localStorage.vendorLogos = val;
                resolve();
            },
            error:function(err){
                reject(err);
            }
        })
    });
    
}