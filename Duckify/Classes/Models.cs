using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Duckify {
    public class SpotifySearchResult {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Length { get; set; }
        public string Artists { get; set; }

        public SpotifySearchResult() {

        }

        public SpotifySearchResult(FullTrack track) {
            Id = track.Id;
            Name = track.Name;
            if (track.Album.Images.Count > 0) {
                ImageUrl = track.Album.Images[0].Url;
            }
            Length = Helper.ConvertMsToReadable(track.DurationMs);
            Artists = track.Artists.Select(x => x.Name).ConvertToString();
        }
    }
}
