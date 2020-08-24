using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Duckify.Models {

    public class UserStarredSongs {
        public string UserId { get; set; }
        public User User { get; set; }
        public string SongId { get; set; }
        public Song Song { get; set; }

    }

    public class Song {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public int Length { get; set; }
        public int PlayCount { get; set; }
        public List<UserStarredSongs> FavoriteBy { get; set; }
    }
}