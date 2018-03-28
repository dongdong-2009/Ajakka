var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
router.use(bodyParser.urlencoded({ extended: true }));
var userController = require('../controllers/userController');

// CREATES A NEW USER
// requires email, pwd
router.post('/', function (req, res) {
	userController.createUser(req.body.name, req.body.pwd).then(function(created){
		res.status(200).send(created);
	},function(error){
		return res.status(500).send("There was a problem adding the information to the database.");
	});
});

// RETURNS ALL THE USERS IN THE DATABASE
router.get('/', function (req, res) {
	userController.findAll(10,0)
	.then(function(users){
		res.status(200).send(users);
	})
	.catch(function(error){
		return res.status(500).send("There was a problem finding the users.");
	});
});

router.get('/pageCount', function (req, res) {
    var pageSize = req.query.pageSize;
    if(!pageSize){
        pageSize = 10;
    }
	userController.getPageCount(pageSize)
	.then(function(pageCount){
		res.status(200).send({pageCount:pageCount});
	})
	.catch(function(error){
		console.log(error);
		return res.status(500).send("There was a problem getting the page count");
	});
});


router.get('/settings/showVendorLogos', function(req,res){
	userController.getSettingsValue('showVendorLogos',req.session.user.id)
	.then(function(result){
		res.status(200).send({content:result});
	})
	.catch(function(err){
		res.status(500).send(err);
	});
});

router.get('/settings/hideSensorColumn', function(req,res){
	userController.getSettingsValue('hideSensorColumn',req.session.user.id)
	.then(function(result){
		res.status(200).send({content:result});
	})
	.catch(function(err){
		res.status(500).send(err);
	});
});



// RETURNS ALL THE USERS IN THE DATABASE
router.get('/:pageNumber/:pageSize', function (req, res) {
	var pageSize = req.params.pageSize;
	var pageNumber = req.params.pageNumber;
	if(!pageSize){
		pageSize = 10;
	}
	if(!pageNumber){
		pageNumber = 0;
	}
	userController.findAll(pageSize, pageNumber)
	.then(function(users){
		res.status(200).send(users);
	})
	.catch(function(error){
		return res.status(500).send("There was a problem finding the users.");
	});
});

// GETS A SINGLE USER FROM THE DATABASE
router.get('/:id', function (req, res) {
	userController.findById(req.params.id)
	.then(function(user){
		res.status(200).send(user);
	})
	.catch(function(error){
		res.status(500).send("There was a problem finding the user. " + error.message);
	});
});

// DELETES A USER FROM THE DATABASE
router.delete('/:id', function (req, res) {

	if(req.session.user.id == req.params.id){
		return res.status(403).send("User cannot delete his own account.");
	}
	userController.deleteUser(req.params.id)
	.then(function(){
		res.status(200).send("User was deleted.");
	})
	.catch(function(err){
		return res.status(500).send("There was a problem deleting the user. " + err.message);
	});
});

// UPDATES A SINGLE USER IN THE DATABASE
// router.put('/:id', function (req, res) {
//
//     User.findByIdAndUpdate(req.params.id, req.body, {new: true}, function (err, user) {
//         if (err) return res.status(500).send("There was a problem updating the user.");
//         res.status(200).send(user);
//     });
// });

//UPDATES USER PASSWORD 
router.put('/password/:name', function (req, res) {
	userController.changeUserPassword(req.params.name, req.body.oldPassword, req.body.newPassword)
	.then(function(){
		res.status(200).send({result:'ok'});
	})
	.catch(function(error){
		return res.status(500).send("There was a problem updating the user. " + error);
	});
});

router.put('/settings/showVendorLogos/:val', function(req,res){
	userController.setSettingsValue('showVendorLogos',req.session.user.id, req.params.val)
	.then(function(){
		res.status(200).send({result:'ok'});
	})
	.catch(function(err){
		res.status(500).send(err);
	});
});

router.put('/settings/hideSensorColumn/:val', function(req,res){
	userController.setSettingsValue('hideSensorColumn',req.session.user.id, req.params.val)
	.then(function(){
		res.status(200).send({result:'ok'});
	})
	.catch(function(err){
		res.status(500).send(err);
	});
});


module.exports = router;