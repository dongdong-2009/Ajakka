function User(id, name, pwdHash){
    this.id = id;
    this.name = name;
    this.pwdHash = pwdHash;
}

module.exports = User;