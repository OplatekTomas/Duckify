var oldData;

function search() {
    $("#searchResults").animate({ maxHeight: '0px', opacity: '0' }, 200, function () {
        $.get('/?handler=Search&query=' + $("#searchBox").val(), function (data) {
            $("#searchResults").animate({ maxHeight: '910px', opacity: '1' }, 200);
            $("#searchResults").html(data);
        });
    });
}


function render() {
    var player = document.getElementById("player");

    $.get('/api/spotify/currentSong', function (data) {
        if (data !== null) {
            if (player.style.height !== 100) {
                player.classList.add("playerFadeIn");
            }
            if (oldData !== data) {
                oldData = data;
                document.getElementById("songName").innerText = data.name;
                document.getElementById("artistNames").innerText = data.artists;
                document.getElementById("indexImageCover").style.backgroundImage = "url(" + data.imageUrl + ")";


            }

        } else if (player.style.height !== 0) {
            //player.classList.add("playerFadeOut");

        }
    });
}