var oldData;

function render() {
    $.get('/api/spotify/currentSong', function (data) {
        if (data !== null) {
            if (oldData !== data) {
                oldData = data;
                document.getElementById("songName").innerText = data.name;
                document.getElementById("artistNames").innerText = data.artists;
                $("#indexImageCover").attr("src", data.imageUrl);

            }

        }
    });
}