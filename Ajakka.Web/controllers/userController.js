const User = require('../model/user');
const uuidv1 = require('uuid/v1');
var passwordHash = require('password-hash');
var mysql = require('mysql');
var config = require('../config/ajakkaConfiguration');

function _validateLogin(name, password, resolve, reject){
    var user = {name:name};
    console.log(user);
    resolve(user);    
}

function _createUser(name, password, resolve, reject){
    console.log('_createUser');
    var id = uuidv1();
    var pwdHash = passwordHash.generate(password);
    var user = new User(id, name, pwdHash);
    
    var connection = createConnection();
    
    var values = [[user.id, user.name, user.pwdHash]];
    
    connection.query('insert into users (id, name, pwdHash) values (?)', values, function (err, result, fields) {
        connection.end();
        console.log(err);
        if(err){
            reject(err);
            return;
        }
        
        resolve(user);
    });

}


function _findAll(resolve, reject){

}

function _findById(id, resolve, reject){
    
}

function _deleteUser(id, resolve, reject){

}

function _changeUserPassword(id, oldPassword, newPassword, resolve, reject){
    
}

function validateLogin(name, password){
    return new Promise(function(resolve, reject){
        _validateLogin(email, password, resolve, reject);
      });
}

function createUser(name, password){
    return new Promise(function(resolve, reject){
        _createUser(name, password, resolve, reject);
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

function createConnection(){
    return mysql.createConnection(config.getMySqlUrl());

}

module.exports.validateLogin = validateLogin;
module.exports.createUser = createUser;
module.exports.findAll = findAll;
module.exports.findById = findById;
module.exports.deleteUser = deleteUser;
module.exports.changeUserPassword = changeUserPassword;