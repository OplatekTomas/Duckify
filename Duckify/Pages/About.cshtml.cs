using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Duckify.Pages {
    public class AboutModel : PageModel {
        public string Message { get; set; }

        public UserManager<IdentityUser> _manager;

        public AboutModel(UserManager<IdentityUser> manager) {
            _manager = manager;
        }

        public void OnGet() {
            GetToken().Wait();
            

        }

        public async Task GetToken() {
            Message = Request.Cookies["SpotifyToken"];
        }
    }
}
