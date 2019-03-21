var songsQueued = null;
var token;

function init() {
    token = spotifyToken;
    api = new SpotifyWebApi();
    api.setAccessToken(token);
    $.get('/api/spotify/currentSong', function (data) {
        if (data.id != null) {
            startPlayback();
        }
    });
    setInterval(renderView, 1500);
}

function waitForElement() {
    if (typeof spotifyToken !== "undefined") {
        init();
    }
    else {
        setTimeout(waitForElement, 100);
    }
}

function renderView() {
    $.get('/Admin/Player?handler=GetQueue', function (data) {
        $("#queueResults").html("")
        $("#queueResults").html(data)
        if (document.getElementById("noSongs") != null) {
            songsQueued = false;
        } else if (songsQueued == false) {
            startPlayback();
            songsQueued = true;
        }
    });

}

function startPlayback() {
    $.get('/api/spotify/currentSong', function (data) {
        var json = {};
        json.uris = ["spotify:track:" + data.id];
        var test = JSON.stringify(json);
        console.log(test);
        api.play(json, function () {
        });
    });
}

function getCookieValue(a) {
    var b = document.cookie.match('(^|;)\\s*' + a + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}


var player;
var api;
const playerName = 'Duckify Web Playback'
window.onSpotifyWebPlaybackSDKReady = () => {
    player = new Spotify.Player({
        name: playerName,
        getOAuthToken: cb => { cb(token); }
    });
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
