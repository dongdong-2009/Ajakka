var express = require('express');
var router = express.Router();

/* GET users listing. */
router.get('/', function(req, res, next) {
  var page = req.query.page;
  res.render('users', { title: 'Ajakka', page:page });
});

module.exports = router;
