function loadUserSettings(){
    return new Promise(function(resolve, reject){
        loadHideSensorColumn()
        .then(function(){
            loadShowVendorLogos()
            .then(function(){
                loadPageSize().then(function(){
                    resolve();  
                })
                .catch(function(err){
                    reject(err);
                });
            })
            .catch(function(err){
                reject(err);
            });
        })
        .catch(function(err){
            reject(err);
        });
    });
    
}

function loadHideSensorColumn(){
    return new Promise(function(resolve, reject){
        if(window.localStorage.hideSensorColumn){
            resolve();
            return;
        }
    
        $.get({
            url:'api/users/settings/hideSensorColumn',
            success:function(result){
                var value = result.content;
                if(value){
                    if(value == "1"){
                        window.localStorage.hideSensorColumn = 1;
                    }
                    else{
                        window.localStorage.hideSensorColumn = 0;
                    }
                    
                }
                else{
                    window.localStorage.hideSensorColumn = 0;
                }
                resolve();
            },
            error:function(err){
                reject(err);
            }
        })
    });
}

function loadShowVendorLogos(){
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
        });
    });
}

function loadPageSize(){
    return new Promise(function(resolve, reject){
        if(window.localStorage.endpointsPageSize){
            resolve();
            return;
        }
        $.get({
            url:'api/users/settings/endpointsPageSize',
            success:function(result){
                let val = result.content;
                if(!val){
                    val = 10;
                }
                window.localStorage.endpointsPageSize = val;
                resolve();
            },
            error:function(err){
                reject(err);
            }
        });
    });
}