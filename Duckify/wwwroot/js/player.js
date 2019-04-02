var songsQueued = null;
var token;


function search() {
    $("#searchResults").animate({ maxHeight: '0px', opacity: '0' }, 200, function () {
        $.get('/?handler=Search&query=' + $("#searchBox").val(), function (data) {
            $("#searchResults").animate({ maxHeight: '910px', opacity: '1' }, 200);
            $("#searchResults").html(data);
        });
    });
}

function init() {
    token = spotifyToken;
    api = new SpotifyWebApi();
    api.setAccessToken(token);
    setInterval(renderView, 1000);
}

function waitForElement() {
    if (typeof spotifyToken !== "undefined") {
        init();
    }
    else {
        setTimeout(waitForElement, 100);
    }
}


function renderQueue() {
    $.get("/api/spotify/qHash", function (hash) {
        if (dataToken !== hash) {
            dataToken = hash;
            renderQueueOverride();
        }
    });
}

var dataToken = "";
function renderView() {
    $.get("/api/spotify/qHash", function (hash) {
        if (dataToken !== hash) {
            dataToken = hash;
            $.get('/Admin/Player?handler=GetQueue', function (data) {
                $("#queueResults").html("");
                $("#queueResults").html(data);
                if (document.getElementById("noSongs") !== null) {
                    songsQueued = false;
                } else if (songsQueued === false) {
                    startPlayback();
                    songsQueued = true;
                }
            });
        }
    });  
}

function startPlayback() {
    $.get('/api/spotify/currentSong', function (data) {
        play(data);
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
    var json = {};
    $.get('/api/spotify/currentSong', function (data) {
        if (data !== null) {
            json.play = true;
        } else {
            json.play = false;
        }
        api.transferMyPlayback(array, json, function (success, data) {
            if (success === null) {
                window.setInterval(function () {
                    player.getCurrentState().then(data => renderUI(data));
                }, 1000);
            }
        });
    });
    
   
}

function pause() {
    player.togglePlay();
}


var origVolume;
function mute() {
    player.getVolume().then(function (data) {
        if (data === null) {
            player.setVolume(origVolume);
            document.getElementById("mute").setAttribute("class", "btn btn-outline-dark btn-sm fas fa-volume-up");

        } else {
            origVolume = data;
            player.setVolume(0);
            document.getElementById("mute").setAttribute("class", "btn btn-outline-dark btn-sm fas fa-volume-mute");

        }
    });
}

function play(data) {
    var json = {};
    json.uris = [data.uri];
    api.play(json, function () {
    });
}

function next() {
    $.get('/Admin/Player?handler=NextSong', function (data) {
        console.log(data);
        if (data !== null) {
            play(data);
        }
    });      
}

function convertTime(millis) {
    var minutes = Math.floor(millis / 60000);
    var seconds = ((millis % 60000) / 1000).toFixed(0);
    return minutes + ":" + (seconds < 10 ? '0' : '') + seconds;
}

function renderUI(data) {
    if (data !== null) {
        $("#playerImageCover").attr("src", data.track_window.current_track.album.images[0].url);
        if (data.paused) {
            document.getElementById("pause").setAttribute("class", "btn btn-outline-dark d-inline btn-sm fas fa-play");
        } else {
            document.getElementById("pause").setAttribute("class", "btn btn-outline-dark d-inline btn-sm fas fa-pause");
        }

        document.getElementById("progressSlider").max = data.duration;
        document.getElementById("progressSlider").value = data.position;
        document.getElementById("songName").innerText = data.track_window.current_track.name;
        document.getElementById("progressText").innerText = convertTime(data.position) +"/"+ convertTime(data.duration);

        document.getElementById("artistNames").innerText = data.track_window.current_track.artists.map(function (elem) {
            return elem.name;
        }).join(", ");
        if ((data.duration - data.position) < 1500) {
            next();
        }
    }
}

function seek(value) {
    document.getElementById("progressSlider").value = value;
    player.seek(value);

}
