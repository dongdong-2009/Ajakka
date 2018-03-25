var express = require('express');
var router = express.Router();
var bodyParser = require('body-parser');
var fs= require('fs');

router.use(bodyParser.urlencoded({ extended: true }));


router.get('/:id', function (req, res) {
    var vendorId = req.params.id;

    var fileName = './images/vendor/' + vendorId;
    if(fs.existsSync(fileName + '.png')){
        var img = fs.readFileSync(fileName + '.png');
        res.writeHead(200, {'Content-Type': 'image/png' });
        res.end(img, 'binary');
        return;
    }
    else if(fs.existsSync(fileName + '.jpg')){
        var img = fs.readFileSync(fileName + '.jpg');
        res.writeHead(200, {'Content-Type': 'image/jpeg' });
        res.end(img, 'binary');
        return;
    }
    var img = fs.readFileSync('./images/def/1pixel.png');
    res.writeHead(200, {'Content-Type': 'image/png' });
    res.end(img, 'binary');
    return;
});


module.exports = router;