using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace Duckify {
    public class Spotify {

        public static SpotifyWebAPI Client { get; set; }
        public static bool IsInitialized { get; set; } = false;

        public static void Init(string token) {
            if (!IsInitialized) {
                SpotifyWebAPI api = new SpotifyWebAPI() {
                    TokenType = "Bearer",
                    AccessToken = token
                };
                IsInitialized = true;
            }
        }

    }
}
