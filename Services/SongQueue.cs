using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duckify.Models;
using SpotifyAPI.Web;

namespace Duckify.Services {
    public class QueueItem {
        public int LikeCount { get; set; } = 1;
        public string SongId { get; set; }
        public FullTrack Track { get; set; }
        public User AddedBy { get; set; }

        public List<User> LikedBy { get; set; } = new List<User>();

        public List<User> DislikedBy { get; set; } = new List<User>();
    }

    public class SongQueue {
        public QueueItem CurrentlyPlaying { get; set; }
        public List<QueueItem> Queue { get; }

        public int Count => Queue.Count;

        private SpotifyService _service;

        public event EventHandler QueueChanged;

        public SongQueue(SpotifyService service) {
            Queue = new List<QueueItem>();
            _service = service;
        }

        public async Task Add(string id, User user) {
            if (Contains(id)) {
                Like(id,user);
                return;
            }
            var track = await _service.Client.Tracks.Get(id);
            var item = new QueueItem {Track = track, AddedBy = user, SongId = track.Id};
            if (Queue.Count == 0) {
                CurrentlyPlaying = item;
            }

            Queue.Add(item);
            QueueChanged?.Invoke("Queue changed", new EventArgs());
        }


        public bool Contains(string id) {
            return Queue.Any(x => x.SongId == id);
        }

        public void Like(string id, User user) {
            var index = Queue.FindIndex(x => x.SongId == id);
            if (Queue.Count == 1) {
                Queue[index].LikeCount++;
                QueueChanged?.Invoke("Queue changed", new EventArgs());
                return;
            }
            Queue[index].LikeCount++;
            var item = Queue[index];
            while (index != 0 && item.LikeCount > Queue[index-1].LikeCount) {
                index--; 
            }
            Queue.Remove(item);
            Queue.Insert(index, item);
            QueueChanged?.Invoke("Queue changed", new EventArgs());

        }
    }
}