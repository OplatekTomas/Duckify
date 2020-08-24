using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Duckify.Models {

    public class UserStarredSongs {
        public int UserId;
        public User User;

        public int SongId;
        public Song Song;

    }

    public class Song {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public int Length { get; set; }
        public int PlayCount { get; set; }
    }
}