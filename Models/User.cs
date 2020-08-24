using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Duckify.Models {
    public class User : IdentityUser {
        public List<UserStarredSongs> FavoriteSongs { get; set; }
    }
}