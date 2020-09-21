using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;

namespace Duckify.Services {
    public class SpotifyService {
        private readonly string _clientId;

        private readonly string _clientSecret;

        private readonly List<string> _scope = new List<string>() {
            "user-read-playback-state user-modify-playback-state user-read-currently-playing streaming", //Required for playback control
            "user-read-private" //Not required right now. Will make use of it later. //TODO: Make use of this
        };

        private readonly string _callbackUri = "https://localhost:5001/spotify-callback";

        public SpotifyAuth Auth { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public SpotifyClient Client { get; set; }

        public bool HasClientIds { get; }

        public SpotifyService(IConfiguration config) {
            _clientId = config["SpotifyID"];
            _clientSecret = config["SpotifySecret"];
            if (_clientId != null && _clientSecret != null) {
                HasClientIds = true;
            }
        }

        public string GetSpotifyLoginRedirectUri() {
            
            var uriBuilder = new UriBuilder("https://accounts.spotify.com/authorize");
            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("client_id", _clientId);
            queryBuilder.Add("response_type", "code");
            queryBuilder.Add("redirect_uri", _callbackUri);
            queryBuilder.Add("scope", String.Join(" ", _scope));
            uriBuilder.Query = queryBuilder.ToString();
            return uriBuilder.ToString();
        }

        public async Task<string> Authenticate(string code) {
            if (IsAuthenticated) {
                return null;
            }

            using var client = new HttpClient();
            var authHeader = Helper.Base64Encode(_clientId + ":" + _clientSecret);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "https://localhost:5001/spotify-callback"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });
            var result = await client.PostAsync("https://accounts.spotify.com/api/token", formContent);
            if (result.StatusCode != HttpStatusCode.OK) {
                IsAuthenticated = false;
                return result.ReasonPhrase;
            }

            var content = await result.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<SpotifyAuth>(content);
            auth.Service = this;
            Auth = auth;
            Client = new SpotifyClient(Auth.AccessToken);

            Auth.EnableRefresh(authHeader);
            IsAuthenticated = true;
            return null;
        }
    }
}