function getCookieValue(a) {
    var b = document.cookie.match('(^|;)\\s*' + a + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}


var player;
var api;
const playerName = 'Duckify Web Playback'
window.onSpotifyWebPlaybackSDKReady = () => {
    const token = spotifyToken;
    player = new Spotify.Player({
        name: playerName,
        getOAuthToken: cb => { cb(token); }
    });
    api = new SpotifyWebApi();
    api.setAccessToken(token);
    // Error handling
    player.addListener('initialization_error', ({ message }) => { console.error(message); });
    player.addListener('authentication_error', ({ message }) => { console.error(message); });
    player.addListener('account_error', ({ message }) => { console.error(message); });
    player.addListener('playback_error', ({ message }) => { console.error(message); });

    // Playback status updates
    player.addListener('player_state_changed', state => { console.log(state); });

    // Ready
    player.addListener('ready', ({ device_id }) => {
        transferPlayback(device_id);
        //console.log('Ready with Device ID', device_id);
    });

    // Not Ready
    player.addListener('not_ready', ({ device_id }) => {
        console.log('Device ID has gone offline', device_id);
    });

    // Connect to the player!
    player.connect();
};

function transferPlayback(device_id) {
    var array = [device_id];
    api.transferMyPlayback(array, null, function (success, data) {
        console.log(success)
        if (success == null) {
            window.setInterval(function () {
                player.getCurrentState().then(data => renderUI(data));
            }, 1000);
        }
    });
}

function renderUI(data) {
    $("#playerImageCover").attr("src", data.track_window.current_track.album.images[0].url)

}
