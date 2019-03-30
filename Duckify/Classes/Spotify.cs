using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Timers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using System.Reflection;
using Duckify.Classes;

namespace Duckify {
    public class Spotify {
        [Serializable]
        public class QueueCollection : KeyedCollection<string, QueueItem> {

            // This is the only method that absolutely must be overridden,
            // because without it the KeyedCollection cannot extract the
            // keys from the items. The input parameter type is the 
            // second generic type argument, in this case OrderItem, and 
            // the return value type is the first generic type argument,
            // in this case int.
            //
            protected override string GetKeyForItem(QueueItem item) {
                // In this example, the key is the part number.
                return item.Id;
            }

        }


        public static string Hash { get; set; }
        public static SpotifyWebAPI Client { get; set; }
        private static QueueCollection SongQueue { get; set; } = new QueueCollection();
        public static bool IsInitialized { get; set; } = false;
        private static Timer _refreshTimer;
        private static Timer _tickTimer;

        public static void Init(string token, string refreshToken) {
            if (!IsInitialized) {
                Client = new SpotifyWebAPI() {
                    TokenType = "Bearer",
                    AccessToken = token
                };
                IsInitialized = true;
                //initializes refresh timer and starts 
                StartRefreshTimer(refreshToken);
                StartTickTimer();
            }
        }

        public static void Terminate() {
            SongQueue = new QueueCollection();
            IsInitialized = false;
            _refreshTimer.Stop();
            _refreshTimer.Close();
            _refreshTimer = null;
            _tickTimer.Stop();
            _tickTimer.Close();
            _tickTimer = null;
            Client = null;
        }

        public static QueueItemResult QueueNextItem(string token) {
            if(SongQueue.Count <= 1) {
                return null;
            }
            SongQueue.Remove(SongQueue[0]);
            return new QueueItemResult(SongQueue[0], token);
        }

        public static QueueItemResult GetCurrentItem(string token) {
            if (SongQueue.Count == 0) {
                return null;
            }
            return new QueueItemResult(SongQueue[0], token);
        }


        public static List<QueueItemResult> GetQueueItems(string token) {
            List<QueueItemResult> results = new List<QueueItemResult>();
            foreach (var item in SongQueue) {
                results.Add(new QueueItemResult(item, token));
            }
            if(results.Count > 0) {
                results.RemoveAt(0);
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
            if (SongQueue.Contains(songId)) {
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
                var first = SongQueue[0];
                SongQueue.RemoveAt(0);
                SongQueue = SongQueue.OrderByDescending(x => x.Likes).ToQueue();
                SongQueue.Insert(0, first);
                return true;
            }
            var track = await Client.GetTrackAsync(songId);
            if (track.HasError()) {
                return false;
            }
            SongQueue.Add(new QueueItem(track, addedBy));
            return true;
        }

        private static void StartTickTimer() {
            if (_tickTimer == null) {
                _tickTimer = new Timer(500);
            }
            if (_tickTimer.Enabled) {
                _tickTimer.Stop();
                _tickTimer.Close();
                _tickTimer = new Timer(500);
            }
            _tickTimer.Elapsed += (s, ev) => Tick();
            _tickTimer.Start();
        }

        private static void Tick() {
            Hash = HashObject.GenerateKey(SongQueue);
        }

        private static void StartRefreshTimer(string refreshToken) {
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
