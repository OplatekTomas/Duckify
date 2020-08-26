using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using Duckify.Services;
using SpotifyAPI.Web;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Duckify {
    public class SpotifyAuth {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        [JsonPropertyName("token_type")] public string TokenType { get; set; }
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        [JsonPropertyName("scope")] public string Scope { get; set; }

        private Timer _tokenRefresh;

        private string _auth;

        public SpotifyService Service {  private get; set; }

        private async Task TimerElapsed() {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _auth);
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("refresh_token", RefreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
            });
            var result = await client.PostAsync("https://accounts.spotify.com/api/token", formContent);
            if (!result.IsSuccessStatusCode) {
                return;
            }

            var newValues = JsonSerializer.Deserialize<SpotifyAuth>(await result.Content.ReadAsStringAsync());
            AccessToken = newValues.AccessToken;
            TokenType = newValues.TokenType;
            ExpiresIn = newValues.ExpiresIn;
            Scope = newValues.Scope;    
            Service.Client = new SpotifyClient(AccessToken);
            EnableRefresh();
        }

        private void EnableRefresh() {
            if (_tokenRefresh != null && _tokenRefresh.Enabled) {
                _tokenRefresh.Stop();
            }

            _tokenRefresh = new Timer(ExpiresIn - 10);
            _tokenRefresh.AutoReset = false;
            _tokenRefresh.Elapsed += async (s, ev) => await TimerElapsed();
            _tokenRefresh.Start();
        }

        public void EnableRefresh(string auth) {
            _auth = auth;
            EnableRefresh();
        }
    }
}