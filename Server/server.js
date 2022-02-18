var io = require('socket.io')(process.env.PORT || 3000);
var shortid = require('shortid');

console.log('server started');

var players = [];

io.on('connection', function(socket){
    var thisClientId = shortid.generate();
    console.log('client connected: ', thisClientId);
    
	socket.on('enterWorld', function(data){
		console.log('entering world');
		if(players.length == 0)
            data.isHost = true;
        else
            data.isHost = false;
        
        data.NetworkId = thisClientId;
        socket.emit('getNetworkId', data);
	});
	
    socket.on('spawnPlayer', function(data){
        console.log('spawning player');
        players.push(data);
		data.NetworkId = thisClientId;
		socket.emit('spawnPlayer', data);
		socket.broadcast.emit('spawnPlayer', data);
    });
    
    socket.on('disconnect', function(){
        players.splice(players.indexOf(thisClientId), 1);
        socket.broadcast.emit('playerDisconnect', {NetworkId: thisClientId});
        console.log('client disconnected');
        if(players.length == 0)
            worldObjects = [];
    });
	
	socket.on('syncWorldRequest', function(data){
		socket.broadcast.emit('syncWorldRequest', data);
	});
	
	socket.on('syncCharacter', function(data){
		data.NetworkId = thisClientId;
		socket.broadcast.emit('syncCharacter', data);
		socket.emit('syncCharacter', data);
	});
	
	socket.on('syncWorld', function(data){
		socket.broadcast.emit('syncWorld', data);
	});
	
	socket.emit('ready');
});