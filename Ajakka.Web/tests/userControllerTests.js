var chai = require('chai');
var expect = chai.expect;
var userController = require('../controllers/userController');
const assert = require('assert');
var mysql = require('mysql');
var config = require('../config/ajakkaConfiguration');


function createConnection(){
    return mysql.createConnection(config.getMySqlUrl());

}

describe('User', function() {
    describe('#findAll()',function(){
        before(function(done){
            var connection = createConnection();
            
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                const makeRequest = async()=>{
                    await userController.createUser('user1','password');
                    await userController.createUser('user2','password');
                    await userController.createUser('user3','password');
                    await userController.createUser('user4','password');
                    await userController.createUser('user5','password');
                    await userController.createUser('user6','password');
                    await userController.createUser('user7','password');
                    await userController.createUser('user8','password');
                    await userController.createUser('user9','password');
                    
                };
                makeRequest().then(function(){
                    done();
                })
                .catch(function(err){
                    done(err);
                });
                
            });
        });

        it('should find first five users',function(done){
            userController.findAll(5,0).then(function(users){
                assert.equal(users.length, 5);
                for(var i=1; i < 6; i++){
                    assert.equal(users[i-1].name,'user' + i);
                    assert.ok(users[i-1].id);
                    assert.equal(users[i-1].pwdHash, '');
                }
                done();
            }).catch(function(err){
                done(err);
            });
        });

        it('should find second page with three users',function(done){
            userController.findAll(3,1).then(function(users){
                assert.equal(users.length, 3);
                for(var i=4; i < 7; i++){
                    assert.equal(users[i-4].name,'user' + i);
                    assert.ok(users[i-4].id);
                    assert.equal(users[i-4].pwdHash, '');
                }
                done();
            }).catch(function(err){
                done(err);
            });
        });

        it('should return empty array',function(done){
            userController.findAll(5,10).then(function(users){
                
                assert.equal(users.length, 0);
                done();

            }).catch(function(err){
                done(err);
            });
        });
    });

    describe('#findById()',function(){
        before(function(done){
            var connection = createConnection();
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                const makeRequest = async()=>{
                    await userController.createUser('user1','password');
                    await userController.createUser('user2','password');
                    await userController.createUser('user3','password');
                    await userController.createUser('user4','password');
                    await userController.createUser('user5','password');
                    await userController.createUser('user6','password');
                    await userController.createUser('user7','password');
                    await userController.createUser('user8','password');
                    await userController.createUser('user9','password');
                    
                };
                makeRequest().then(function(){
                    done();
                })
                .catch(function(err){
                    done(err);
                });
                
            });
        });

        it('should find user by id', function(done){
            userController.findAll(3,1).then(function(users){
                var idToFind = users[2].id;
                userController.findById(idToFind).then(function(user){
                    assert.equal(user.name, users[2].name);
                    assert.equal(user.id, users[2].id);
                    assert.equal(user.pwdHash, '');
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });

        it('should not find user by id', function(done){
            userController.findById('1234').then(function(user){
                assert.equal(user, null);
                done();
            })
            .catch(function(err){
                done(err);
            });
        });

        it('should not find user (sql injection attack)', function(done){
            userController.findAll(3,1).then(function(users){
                var idToFind = users[2].id;
                userController.findById('\' or 1=1 or 1=\'').then(function(user){
                    assert.equal(user, null);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });

        it('should not drop table users (sql injection attack)', function(done){
            userController.findAll(3,1).then(function(users){
                var idToFind = users[2].id;
                userController.findById(';drop table users;--').then(function(user){
                    assert.equal(user, null);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });
    });

    describe('#findByName()',function(){
        before(function(done){
            var connection = createConnection();
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                const makeRequest = async()=>{
                    await userController.createUser('user1','password');
                    await userController.createUser('user2','password');
                    await userController.createUser('user3','password');
                    await userController.createUser('user4','password');
                    await userController.createUser('user5','password');
                    await userController.createUser('user6','password');
                    await userController.createUser('user7','password');
                    await userController.createUser('user8','password');
                    await userController.createUser('user9','password');
                    
                };
                makeRequest().then(function(){
                    done();
                })
                .catch(function(err){
                    done(err);
                });
                
            });
        });

        it('should find user by name', function(done){
            userController.findAll(3,1).then(function(users){
                var nameToFind = users[2].name;
                userController.findByName(nameToFind).then(function(user){
                    assert.equal(user.name, users[2].name);
                    assert.equal(user.id, users[2].id);
                    assert.equal(user.pwdHash, '');
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });

        it('should not find user by nameToFind', function(done){
            userController.findByName('1234').then(function(user){
                assert.equal(user, null);
                done();
            })
            .catch(function(err){
                done(err);
            });
        });

        it('should not find user (sql injection attack)', function(done){
            userController.findAll(3,1).then(function(users){
                var idToFind = users[2].id;
                userController.findByName('\' or 1=1 or 1=\'').then(function(user){
                    assert.equal(user, null);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });

        it('should not drop table users (sql injection attack)', function(done){
            userController.findAll(3,1).then(function(users){
                var idToFind = users[2].id;
                userController.findByName(';drop table users;--').then(function(user){
                    assert.equal(user, null);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });
    });

    describe('#create()', function() {
        beforeEach(function(done){ 
            var connection = createConnection();
            
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                done();
            });
           
        });
        
        it('should create user', function(done) {
            userController.createUser('name','password')
            .then(function(user){
                assert.equal(user.name, 'name');
                assert.ok(user.pwdHash);
                assert.ok(user.id);
                var connection = createConnection();
                connection.query('select id,name,pwdHash from users', function (err, result, fields) {
                    
                    connection.end();
                    
                    assert.equal(user.name, result[0].name);
                    assert.equal(user.pwdHash, result[0].pwdHash);
                    assert.equal(user.id, result[0].id);
                    done();
                });
               
            })
            .catch((reason)=>{done(reason);});
        });

        it('should not set passwordHash to test (sql injection attack)', function(done) {
            userController.createUser('test\',\'test\')--','password')
            .then(function(user){;
                var connection = createConnection();
                connection.query('select id,name,pwdHash from users', function (err, result, fields) {
                    
                    connection.end();
                    assert.notEqual(result[0],'test');
                    assert.equal(user.name, result[0].name);
                    assert.equal(user.pwdHash, result[0].pwdHash);
                    assert.equal(user.id, result[0].id);
                    done();
                });
               
            })
            .catch((reason)=>{done(reason);});
        });
    });

    
});

