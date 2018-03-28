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
    describe('searching',function(){
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

        describe('#findAll()',function(){
            
    
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

        describe('getPageCount()',function(){
            it('should return 3 pages',function(done){
                userController.getPageCount(3)
                .then(function(pageCount){
                    assert.equal(pageCount, 3);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            });
        });

        describe('getPageCount()',function(){
            it('should return 2 pages',function(done){
                userController.getPageCount(5)
                .then(function(pageCount){
                    assert.equal(pageCount, 2);
                    done();
                })
                .catch(function(err){
                    done(err);
                });
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

    describe('#deleteUser()',function(){
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

        it('should delete user by id', function(done){
            userController.findAll(3,1).then(function(users){
                var idToDelete = users[2].id;
                userController.deleteUser(idToDelete).then(function(){
                    userController.findById(idToDelete).then(function(user){
                        assert.equal(user,null);
                        done();
                    })
                    .catch(function(err){
                        done(err);
                    });
                })
                .catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
            
        });

        it('should throw error when user does not exist', function(done){
            
            var idToDelete = '1234';
            userController.deleteUser(idToDelete)
            .then(function(){
                done('function call should have not succeeded');
                })
            .catch(function(err){
                assert.equal(err.message,'User with this id does not exist');
                done();
            });
        });
            

        it('should not delete all users (sql injection attack)', function(done){
            userController.deleteUser('\' or 1=1 or 1=\'').then(function(){
                userController.findAll(3,1).then(function(users){
                    assert.equal(users.length, 3);
                    done();
                }).catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                if(err.message == 'User with this id does not exist'){
                    done();
                    return;
                }
                done(err);
            });
        });
    });

    describe('#validatePassword()',function(){
        before(function(done){
            var connection = createConnection();
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                const makeRequest = async()=>{
                    await userController.createUser('user1','password1');
                    await userController.createUser('user3','');
                };
                makeRequest().then(function(){
                    done();
                })
                .catch(function(err){
                    done(err);
                });
                
            });
        });

        it('should validate user password', function(done){
            userController.validateLogin('user1','password1').then(function(user){
                assert.equal(user.name,'user1');
                done();
            })
            .catch(function(err){
                done(err);
            });
        });

        it('should not validate user password when password is incorrect', function(done){
            userController.validateLogin('user1','password2').then(function(user){
                done(user);
            })
            .catch(function(err){
                assert.equal(err.message, 'User name or password is not valid');
                done();
            });
        });

        it('should not validate user password when user name is incorrect', function(done){
            userController.validateLogin('user2','password1').then(function(user){
                done(user);
            })
            .catch(function(err){
                assert.equal(err.message, 'User name or password is not valid');
                done();
            });
        });

        it('should validate user password password is empty', function(done){
            userController.validateLogin('user3','').then(function(user){
                done(user);
            })
            .catch(function(err){
                assert.equal(err.message, 'User name or password is not valid');
                done();
            });
        });

        it('should validate password when pwdHash is null', function(done){
            var connection = createConnection();
    
            var values = [['admin', 'admin', null]];
            
            connection.query('insert into users (id, name, pwdHash) values (?)', values, function (err, result, fields) {
                connection.end();
                if(err){
                    done(err);
                    return;
                }
                userController.validateLogin('admin','').then(function(user){
                    assert.equal(user.name,'admin');
                    done();
                })
                .catch(function(err){
                    done(err);
                });
            });
        });
    });

    describe('#setSettingsValue()', function(){
        before(function(done){
            var connection = createConnection();
            connection.query('delete from usersettings;', function(err, result, fields){
                if(err){
                    throw err;
                }
                connection.query('insert into usersettings values (\'private\',\'admin\',\'val\')', function(err2, result, fields){
                    if(err2){
                        throw err2;
                    }
                    connection.end();
                    done();
                });
            });
        });

        it('should get an existing settings item', function(done){
            userController.getSettingsValue('private','admin')
            .then(function(result){
                assert.equal(result,'val');              
                done();      
            })
            .catch(function(err){done(err);});
        });

        it('should return null when settings item does not exist', function(done){
            userController.getSettingsValue('pr1vate','admin')
            .then(function(result){
                assert.equal(result, null);              
                done();      
            })
            .catch(function(err){done(err);});
        });

        it('should save a new setting value', function(done){
            userController.setSettingsValue('newKey','newUser','1')
            .then(function(){
                userController.getSettingsValue('newKey','newUser')
                .then(function(result){
                    assert.equal(result,'1');              
                    done();      
                })
                .catch(function(err){done(err);});
            })
            .catch(function(err){
                done(err);
            })
        });

        it('should update an existing setting value', function(done){
            userController.setSettingsValue('private','admin','30')
            .then(function(){
                userController.getSettingsValue('private','admin')
                .then(function(result){
                    assert.equal(result,'30');              
                    done();      
                })
                .catch(function(err){done(err);});
            })
            .catch(function(err){
                done(err);
            })
        });
    });

    describe('#changeUserPassword()',function(){
        before(function(done){
            var connection = createConnection();
            connection.query('delete from users', function (err, result, fields) {
                if (err){ 
                    throw err;
                }
                connection.end();
                const makeRequest = async()=>{
                    await userController.createUser('user1','oldPassword');
                };
                makeRequest().then(function(){
                    done();
                })
                .catch(function(err){
                    done(err);
                });
                
            });
        });

        it('should change user password', function(done){
            userController.changeUserPassword('user1','oldPassword','newPassword').then(function(user){
                userController.validateLogin('user1','newPassword').then(function(user){
                    assert.equal(user.name,'user1');
                    done();
                }).catch(function(err){
                    done(err);
                });
            })
            .catch(function(err){
                done(err);
            });
        });

        it('should not change user password when old password is incorrect', function(done){
            userController.changeUserPassword('user1','oldPassword2','newPassword').then(function(user){
               done('password change should have failed');
            })
            .catch(function(err){
                assert.equal(err.message,'User name or password is not valid');
                done();
            });
        });

        it('should not validate user password when user name is incorrect', function(done){
            userController.changeUserPassword('user2','oldPassword','newPassword').then(function(user){
                done('password change should have failed');
            })
            .catch(function(err){
                assert.equal(err.message,'User name or password is not valid');
                done();
            });
        });
    });
});

