using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;

namespace Duckify {
    public class Spotify {

        public static SpotifyWebAPI Client { get; set; }
        private static Dictionary<string, QueueItem> SongQueue { get; set; } = new Dictionary<string, QueueItem>();
        public static bool IsInitialized { get; set; } = false;
        private static Timer _refreshTimer;

        public static void Init(string token, string refreshToken) {
            if (!IsInitialized) {
                Client = new SpotifyWebAPI() {
                    TokenType = "Bearer",
                    AccessToken = token
                };
                IsInitialized = true;
                //initializes refresh timer and starts 
                StartTimer(refreshToken);
            }
        }

        public static void Terminate() {
            IsInitialized = false;
            _refreshTimer.Stop();
            _refreshTimer.Close();
            _refreshTimer = null;
            Client = null;
        }

        public static List<QueueItemResult> GetQueueItems(string token) {
            List<QueueItemResult> results = new List<QueueItemResult>();
            foreach (var item in SongQueue.Values) {
                results.Add(new QueueItemResult(item, token));
            }
            return results;
        }

        public static async Task<List<SpotifySearchResult>> SearchTracks(string query) {
            var apiResult = await Client.SearchItemsAsync(query, SearchType.Track, 10);
            if (apiResult.Error != null || apiResult.Tracks.Error != null) {
                return new List<SpotifySearchResult>();
            }
            List<SpotifySearchResult> results = new List<SpotifySearchResult>();
            foreach (var item in apiResult.Tracks.Items) {
                results.Add(new SpotifySearchResult(item));
            }
            return results;
        }

        public async static Task<bool> AddToQueue(string songId, string addedBy) {
            if (songId == null) {
                return false;
            }
            if (SongQueue.ContainsKey(songId)) {
                if (SongQueue[songId].LikedBy.Contains(addedBy)) {
                    SongQueue[songId].Likes--;
                    SongQueue[songId].LikedBy.Remove(addedBy);
                    if (SongQueue[songId].Likes == 0) {
                        SongQueue.Remove(songId);
                    }


                } else {
                    SongQueue[songId].Likes++;
                    SongQueue[songId].LikedBy.Add(addedBy);
                }
                SongQueue = SongQueue.OrderByDescending(x => x.Value.Likes).ToDictionary(x => x.Key, x => x.Value);
                return true;
            }
            var track = await Client.GetTrackAsync(songId);
            if (track.HasError()) {
                return false;
            }
            SongQueue.Add(songId, new QueueItem(track, addedBy));
            return true;
        }

        private static void StartTimer(string refreshToken) {
            //59 minutes and 30 seconds in miliseconds (((60 seconds * 59 minutes) + 30 seconds) * 1000 ms)     
            if (_refreshTimer == null) {
                _refreshTimer = new Timer(3570 * 1000);
            }
            if (_refreshTimer.Enabled) {
                _refreshTimer.Stop();
                _refreshTimer.Close();
                _refreshTimer = new Timer(3570 * 1000);
            }
            _refreshTimer.Elapsed += async (s, ev) => await RefreshToken(refreshToken);
            _refreshTimer.Start();
        }

        private async static Task RefreshToken(string refreshToken) {
            using (HttpClient client = new HttpClient()) {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                //Build url encoded body
                var content = new StringContent("grant_type=refresh_token&refresh_token=" + refreshToken);
                message.Content = content;
                //Refresh header to correct value, yes this probably could be simplified.
                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                //Add Base64 encoded API info
                message.Headers.Add("Authorization", "Basic " + Helper.EncodeTo64("146234f4fccf47ffbe4de27b8b472ce8:d4fcb4f8e79c4a909db89812710111b9"));
                var response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode) {
                    //Get response string, parse it and set the result.
                    var result = await response.Content.ReadAsStringAsync();
                    Client.AccessToken = JObject.Parse(result)["access_token"].ToString();
                }
            }
        }
    }
}
