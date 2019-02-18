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
            var cookies = Request.Cookies.ToDictionary(x => x.Key, x => x.Value);
            var user = await _manager.GetUserAsync(User);
            Message = user.GetSpotifyToken();
        }
    }
}
