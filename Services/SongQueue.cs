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
    }

    public class QueueChangedEventArgs : EventArgs {
        public enum EventTypes {
            SongLiked,
            SongAdded,
            SongRemoved,
            PlayingSongChanged
        }

        public EventTypes ChangeType { get; set; }

        public QueueChangedEventArgs() { }

        public QueueChangedEventArgs(EventTypes type) {
            ChangeType = type;
        }
    }

    public class SongQueue {
        public QueueItem CurrentlyPlaying { get; set; }
        public List<QueueItem> Queue { get; }

        public int Count => Queue.Count;

        private SpotifyService _service;

        public event EventHandler<QueueChangedEventArgs> QueueChanged;

        public SongQueue(SpotifyService service) {
            Queue = new List<QueueItem>();
            _service = service;
        }

        public async Task Add(string id, User user) {
            if (Contains(id)) {
                Like(id, user);
                return;
            }

            var track = await _service.Client.Tracks.Get(id);
            var item = new QueueItem {Track = track, AddedBy = user, SongId = track.Id};
            item.LikedBy.Add(user);
            if (Queue.Count == 0 && CurrentlyPlaying == null) {
                CurrentlyPlaying = item;
                QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.PlayingSongChanged));
            }
            else {
                Queue.Add(item);
                QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.SongAdded));
            }

        }

        public bool UserLikes(string id, User user) {
            var index = Queue.FindIndex(x => x.SongId == id);
            return index >= 0 && Queue[index].LikedBy.Contains(user);
        }


        public bool Contains(string id) {
            return Queue.Any(x => x.SongId == id);
        }

        public void Like(string id, User user) {
            var index = Queue.FindIndex(x => x.SongId == id);
            if (Queue[index].LikedBy.Contains(user)) {
                UnlikeIndexed(index, user);
                return;
            }

            Queue[index].LikeCount++;
            Queue[index].LikedBy.Add(user);
            if (index == 0) {
                QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.SongLiked));
                return;
            }

            var item = Queue[index];
            while (index != 0 && item.LikeCount > Queue[index - 1].LikeCount) {
                index--;
            }

            Queue.Remove(item);
            Queue.Insert(index, item);
            QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.SongLiked));
        }
        
        public void Unlike(string id, User user) {
            if (!Queue.Any(x => x.LikedBy.Contains(user))) {
                return;
            }
            var index = Queue.FindIndex(x => x.SongId == id);
            UnlikeIndexed(index, user);
        }
        
        private void UnlikeIndexed(int index, User user) {
            if (Queue[index].LikeCount <= 1) {
                Queue.RemoveAt(index);
                QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.SongRemoved));
                return;
            }

            Queue[index].LikeCount--;
            Queue[index].LikedBy.Remove(user);
            QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.SongLiked));
        }

        public void Next() {
            if (Queue.Count == 0) {
                CurrentlyPlaying = null;
            }
            else {
                CurrentlyPlaying = Queue[0];
                Queue.RemoveAt(0);
            }

            QueueChanged?.Invoke(this, new QueueChangedEventArgs(QueueChangedEventArgs.EventTypes.PlayingSongChanged));
        }
    }
}