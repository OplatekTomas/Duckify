var oldData;

function render() {
    var player = document.getElementById("player");

    $.get('/api/spotify/currentSong', function (data) {
        if (data !== null) {
            player.style.display = "block";
            if (oldData !== data) {
                oldData = data;
                document.getElementById("songName").innerText = data.name;
                document.getElementById("artistNames").innerText = data.artists;
                $("#indexImageCover").attr("src", data.imageUrl);

            }

        } else {
            player.style.display = "none";
        }
    });
}