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
    var id = uuidv1();
    var pwdHash = passwordHash.generate(password);
    var user = new User(id, name, pwdHash);
    
    var connection = createConnection();
    
    var values = [[user.id, user.name, user.pwdHash]];
    
    connection.query('insert into users (id, name, pwdHash) values (?)', values, function (err, result, fields) {
        connection.end();
        if(err){
            reject(err);
            return;
        }
        
        resolve(user);
    });

}


function _findAll(pageSize, pageNumber, resolve, reject){
    var connection = createConnection();
    var offset = pageSize * pageNumber;
    var query = 'select * from users order by name limit '+ pageSize + ' offset ' + offset;
   
    connection.query(query, function(err,result,fields){
        if(err){
            reject(err);
            return;
        }
        connection.end();
        var users = new Array();
        result.forEach(function(item){
            var user = new User(item.id, item.name, '');
            users.push(user);
        });
        resolve(users);
    });
}

function _findById(id, resolve, reject){
    var connection = createConnection();
    var query = 'select * from users where id=?';
    connection.query(query, id, function(err,result,fields){
        if(err){
            reject(err);
            return;
        }
        connection.end();
        if(result.length == 0){
            resolve(null);
            return;
        }
        var user = new User(result[0].id,result[0].name,'');
        resolve(user);
    });
}

function _findByName(name, resolve, reject){
    var connection = createConnection();
    var query = 'select * from users where name=?';
    connection.query(query, name, function(err,result,fields){
        if(err){
            reject(err);
            return;
        }
        connection.end();
        if(result.length == 0){
            resolve(null);
            return;
        }
        var user = new User(result[0].id,result[0].name,'');
        resolve(user);
    });
}

function _deleteUser(id, resolve, reject){
    var connection = createConnection();
    var query = 'delete from users where id=?';
    var q = connection.query(query, id, function(err,result,fields){
        if(err){
            reject(err);
            return;
        }
        connection.end();
        resolve(id);
    });
}

function _changeUserPassword(id, oldPassword, newPassword, resolve, reject){
    reject('not implemented');
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

function findAll(pageSize, pageNumber){
    return new Promise(function(resolve, reject){
        _findAll(pageSize, pageNumber, resolve, reject);
    })
}

function findById(id){
    return new Promise(function(resolve, reject){
        _findById(id, resolve, reject);
    })
}

function findByName(name){
    return new Promise(function(resolve, reject){
        _findByName(name, resolve, reject);
    });
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
module.exports.findByName = findByName;