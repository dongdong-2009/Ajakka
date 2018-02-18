var express = require('express');
var path = require('path');
//var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var helmet = require('helmet')
var session = require('express-session')

var index = require('./routes/index');
var users = require('./routes/users');
var login = require('./routes/login');
var about = require('./routes/about');

const uuidv1 = require('uuid/v1');

var endpointApi = require('./api/endpoint');
var userApi = require('./api/user');

var app = express();


// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'pug');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(helmet());
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));


app.use(session({
  secret:'double o seven',
  cookie:{secure: false,
     httpOnly: true,
     maxAge:1000*60*60*12
  },
  genid: function(req) {
    return uuidv1() // use UUIDs for session IDs
  },
  resave:true,
  saveUninitialized:true
}));

app.use('/', login);
app.use('/logout', logout);
app.use('/index',requireLogin, index);
app.use('/users',requireLogin, users);
app.use('/about', requireLogin, about)
app.use('/login', login);
app.use('/api/endpoints', blockApi, endpointApi);
app.use('/api/users',blockApi, userApi);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

function blockApi(req,res,next)
{
  if (!req.session.user) {
    var err = new Error('Not Authorized');
    err.status = 401;
    next(err);
  }
  next();
}

function requireLogin (req, res, next) {
  if (!req.session.user) {
    res.redirect('/login');
    return;
  }
  next();

};

function logout(req,res,next)
{
  req.session.destroy();
  res.redirect('/');
}


module.exports = app;
