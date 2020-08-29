using System.Linq;
using System.Text.Json;
using Duckify.Services;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace Duckify.Controllers {
    public class Api : Controller {

        private SongQueue _queue;
        
        public Api(SongQueue queue) {
            _queue = queue;
        } 

        [Route("/api/current_song")]
        public IActionResult CurrentSong() {
            var song = _queue.CurrentlyPlaying;
            return Content(song == null ? "{}" : JsonSerializer.Serialize(new ApiTrack(song.Track, song.LikeCount)));
        }

        [Route("/api/queue")]
        public IActionResult Queue() {
            var songs = _queue.Queue.Select(x => new ApiTrack(x.Track, x.LikeCount));
            return Content(JsonSerializer.Serialize(songs));
        }


        private class ApiTrack {
            public string SongId { get; set; }
            public string  Name { get; set; }
            public int Length { get; set; }
            public string Artist { get; set; }
            public string ImageUrl { get; set; }
            public int LikeCount { get; set; }


            public ApiTrack(FullTrack track, int likeCount) {
                SongId = track.Id;
                LikeCount = likeCount;
                Name = track.Name;
                Length = track.DurationMs;
                Artist = Helper.ConvertArtists(track.Artists);
                ImageUrl = track.Album.Images[0].Url;

            }
        }
        
        
    }
}