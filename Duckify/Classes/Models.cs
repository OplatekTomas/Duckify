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
        public string Uri { get; set; }


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
            Uri = track.Uri;
        }
    }

    public class QueueItemResult : SpotifySearchResult {
        public int Likes { get; set; }

        public bool LikedByUser { get; set; }

        public QueueItemResult() {

        }

        public QueueItemResult(QueueItem item, string token) {
            var track = item.Track;
            Id = track.Id;
            Name = track.Name;
            if (track.Album.Images.Count > 0) {
                ImageUrl = track.Album.Images[0].Url;
            }
            Length = Helper.ConvertMsToReadable(track.DurationMs);
            Artists = track.Artists.Select(x => x.Name).ConvertToString();
            Likes = item.Likes;
            LikedByUser = item.LikedBy.Contains(token);
            Uri = item.Track.Uri;
        }
    }

    public class QueueItem {
        public string Id;
        public FullTrack Track { get; set; }
        public string AddedBy { get; set; }
        public List<string> LikedBy { get; set; } = new List<string>();
        public int Likes { get; set; }
        public string Length { get; set; }

        public QueueItem(FullTrack track, string addedBy = null) {
            Id = track.Id;
            Track = track;
            Length = Helper.ConvertMsToReadable(track.DurationMs);
            addedBy = addedBy ?? "Anon";
            AddedBy = addedBy;
            LikedBy.Add(addedBy);
            Likes = 1;
        }
    }

}


