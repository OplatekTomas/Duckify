using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;

namespace Duckify.Models {
    public class User : IdentityUser {
        public List<UserStarredSongs> FavoriteSongs { get; set; }

        public string AccessCode { get; set; }

        public bool IsTemp { get; set; } = false;

        public DateTime ValidUntil { get; set; }
    }
}