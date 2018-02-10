
function _validateLogin(email, password, resolve, reject){
    var user = {email:email};
    console.log(user);
    resolve(user);    
}

function validateLogin(email, password){
    return new Promise(function(resolve, reject){
        _validateLogin(email, password, resolve, reject);
      });
}


module.exports.validateLogin = validateLogin;