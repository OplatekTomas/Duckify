using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Duckify.Pages.Admin {

    [Authorize(Roles = "Admin")]
    public class PlayerModel : PageModel {

        public PlayerModel(SignInManager<IdentityUser> signInManager) {
            _signInManager = signInManager;
        }

        public SignInManager<IdentityUser> _signInManager;

        public void OnGet() {

        }


        public IActionResult OnPostSpotifyLogin(string returnUrl = null) {
            var redirectUrl = Url.Page("./Player", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Spotify", redirectUrl);
            return new ChallengeResult("Spotify", properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null) {
            if (remoteError != null) {
                //TODO: fill this
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) {
                //TODO: also fill this while you are at it..
            }
            var token = info.AuthenticationTokens.First(x => x.Name == "access_token").Value;
            Response.Cookies.Append("SpotifyToken", token, new CookieOptions { Secure = true, HttpOnly = true });
            Spotify.Init(token); ; ;
            return LocalRedirect("/Admin/Player");
        }

        public string GetToken() {
            return Request.Cookies["SpotifyToken"];
        }
    }
}