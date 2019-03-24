var oldData;

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
                $("#indexImageCover").attr("src", data.imageUrl);

            }

        } else if (player.style.height !== 0) {
            //player.classList.add("playerFadeOut");

        }
    });
}