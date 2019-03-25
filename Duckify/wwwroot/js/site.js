// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

var light = "/lib/bootstrap/dist/css/bootstrap-light.css";
var dark = "/lib/bootstrap/dist/css/bootstrap-dark.css";
var darkTheme = null;

function switchTheme() {
    if (darkTheme === null) {
        darkTheme = partialTheme === "True" ? true : false;
    }
    if (!darkTheme) {
        $('link[href="' + light + '"]').attr('href', dark);
        document.getElementById("themeSwitcher").innerText = "Light theme";
    } else {
        $('link[href="' + dark + '"]').attr('href', light);
        document.getElementById("themeSwitcher").innerText = "Dark theme";

    }
    darkTheme = !darkTheme;
    document.cookie = "DarkTheme=" + darkTheme;

}

function likeSong(id) {
    $("#searchResults").animate({ maxHeight: '0px', opacity: '0' }, 200, function () {
        document.getElementById("searchResults").innerHTML = null;
    });
    $.get('?handler=AddSong&id=' + id, function (data) {

    });
}


