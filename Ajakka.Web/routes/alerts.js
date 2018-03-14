var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res, next) {
  var page = req.query.page;
  res.render('alerts', { title: 'Ajakka', page:page, currentUserName:req.session.user.name });
});

module.exports = router;
