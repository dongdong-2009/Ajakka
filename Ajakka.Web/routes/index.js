var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  var page = req.query.page;
  console.log(req.query);
  res.render('index', { title: 'Ajakka', page:page });
});

module.exports = router;
