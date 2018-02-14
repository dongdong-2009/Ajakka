const User = require('../model/user');
const uuidv1 = require('uuid/v1');
var passwordHash = require('password-hash');

function _validateLogin(email, password, resolve, reject){
    var user = {email:email};
    console.log(user);
    resolve(user);    
}

function _createUser(email, password, resolve, reject){
    var id = uuidv1();
    var pwdHash = passwordHash.generate(password);
    var user = new User(id, email, pwdHash);

    resolve(user);
}

function _findAll(resolve, reject){

}

function _findById(id, resolve, reject){
    
}

function _deleteUser(id, resolve, reject){

}

function _changeUserPassword(id, oldPassword, newPassword, resolve, reject){
    
}

function validateLogin(email, password){
    return new Promise(function(resolve, reject){
        _validateLogin(email, password, resolve, reject);
      });
}

function createUser(email, password){
    return new Promise(function(resolve, reject){
        _createUser(email, password, resolve, reject);
    })
}

function findAll(){
    return new Promise(function(resolve, reject){
        _findAll(resolve, reject);
    })
}

function findById(id){
    return new Promise(function(resolve, reject){
        _findById(id, resolve, reject);
    })
}

function deleteUser(id){
    return new Promise(function(resolve, reject){
        _deleteUser(id, resolve, reject);
    })
}

function changeUserPassword(id, oldPassword, newPassword){
    return new Promise(function(resolve, reject){
        _changeUserPassword(id, oldPassword, newPassword, resolve, reject);
    })
}

module.exports.validateLogin = validateLogin;
module.exports.createUser = createUser;
module.exports.findAll = findAll;
module.exports.findById = findById;
module.exports.deleteUser = deleteUser;
module.exports.changeUserPassword = changeUserPassword;