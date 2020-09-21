
var playerInitialized;

function createDevice(token){
    if(playerInitialized === true){
        return;
    }
    const player = new Spotify.Player({
        name: 'Duckify Web',
        getOAuthToken: cb => { cb(token); }
    });

    // Error handling
    player.addListener('initialization_error', ({ message }) => { console.error(message); });
    player.addListener('authentication_error', ({ message }) => { console.error(message); });
    player.addListener('account_error', ({ message }) => { console.error(message); });
    player.addListener('playback_error', ({ message }) => { console.error(message); });

    

    // Ready
    player.addListener('ready', ({ device_id }) => {
        DotNet.invokeMethodAsync('Duckify', 'InformPlayback', device_id);
                 playerInitialized = true;
        console.log('Ready with Device ID', device_id);
    });
    player.addListener('player_state_changed', state => { console.log(state); });

    // Not Ready
    player.addListener('not_ready', ({ device_id }) => {
        console.log('Device ID has gone offline', device_id);
    });

    // Connect to the player!
    player.connect();
    
}