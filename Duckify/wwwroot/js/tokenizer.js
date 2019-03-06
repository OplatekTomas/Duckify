
function runGenerator() {
    var cookie = getCookie(".AspNet.Consent");
    setUniqueToken().done(function (result) {
        if (cookie !== null && !result) {
            alert("Server was not able to authenticate you (Sorry). This means you won't be able to like or add songs");
        }
    });


}

//I went full egyptian with this one. Do I feel ashamed? Yes.
function setUniqueToken() {
    getUserIP(function (ip) {
        var parts = ip.split(".");
        //Filter out IPv6
        if (parts[0] > 0 && parts[0] < 256) {
            //Call ipify API for my public IP address. 
            $.get('https://api.ipify.org?format=json', function (ipData, status) {
                //Ask server for a aes key and aes initialization vector.
                $.get("?handler=Key", function (data) {
                    var token = ip + ":" + ipData.ip;
                    var enc = Crypt(token, data.key, data.vector);
                    //Ask server to authenticate with encrypted token
                    $.get("?handler=Authenticate&token=" + enc, function (wasAuth) {
                        return wasAuth;
                    });
                });
            });
        }
    });
}


function Crypt(token, keyString, ivString) {
    var key = CryptoJS.enc.Utf8.parse(keyString);
    var iv = CryptoJS.enc.Utf8.parse(ivString);
    var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(token), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
    return encrypted.toString();
}


function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else {
        begin += 2;
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
            end = dc.length;
        }
    }
    return decodeURI(dc.substring(begin + prefix.length, end));
}



function getUserIP(onNewIP) { //  onNewIp - your listener function for new IPs
    //compatibility for firefox and chrome
    var myPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
    var pc = new myPeerConnection({
        iceServers: []
    }),
        noop = function () { },
        localIPs = {},
        ipRegex = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/g,
        key;

    function iterateIP(ip) {
        if (!localIPs[ip]) onNewIP(ip);
        localIPs[ip] = true;
    }

    //create a bogus data channel
    pc.createDataChannel("");

    // create offer and set local description
    pc.createOffer().then(function (sdp) {
        sdp.sdp.split('\n').forEach(function (line) {
            if (line.indexOf('candidate') < 0) return;
            line.match(ipRegex).forEach(iterateIP);
        });

        pc.setLocalDescription(sdp, noop, noop);
    }).catch(function (reason) {
        // An error occurred, so handle the failure to connect
    });

    //listen for candidate events
    pc.onicecandidate = function (ice) {
        if (!ice || !ice.candidate || !ice.candidate.candidate || !ice.candidate.candidate.match(ipRegex)) return;
        ice.candidate.candidate.match(ipRegex).forEach(iterateIP);
    };
}