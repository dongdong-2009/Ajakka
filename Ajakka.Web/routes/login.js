var express = require('express');
var router = express.Router();
var userController = require('../controllers/userController');
var passwordHash = require('password-hash');

/* GET home page. */
router.get('/', function(req, res, next) {
  renderResult(res);
});

router.post('/', function(req, res, next){
  userController.validateLogin(req.body.name, req.body.password)
  .then(function(user){
    req.session.user = {name:user.name, id:user.id};
    res.redirect('/index');
  })
  .catch(function(error){
    renderError(res, error.message);
    return;
  });
});


function renderError(res, error)
{
  res.render('login', { error:error });
}

function renderResult(res)
{
  renderError(res,'');
}

module.exports = router;