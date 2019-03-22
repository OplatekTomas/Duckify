using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duckify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Duckify.Pages.Admin {

    [Authorize(Roles = "Admin")]
    public class PlayerModel : PageModel {

        public PlayerModel(SignInManager<IdentityUser> signInManager) {
            _signInManager = signInManager;
        }

        public SignInManager<IdentityUser> _signInManager;


        [BindProperty]
        public string SearchText { get; set; }

        [BindProperty]
        public List<FullTrack> SearchResult { get; set; }

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
            var refreshToken = info.AuthenticationTokens.First(x => x.Name == "refresh_token").Value;
            Response.Cookies.Append("SpotifyToken", token, new CookieOptions { Secure = true, HttpOnly = true });
            Spotify.Init(token, refreshToken);
            return LocalRedirect("/Admin/Player");
        }

        public IActionResult OnPostSpotifyLogout() {
            Spotify.Terminate();
            return LocalRedirect("/Admin/Player");
        }

        public async Task<PartialViewResult> OnGetSearch(string query) {
            var result = await Spotify.SearchTracks(query);
            var partialView = new PartialViewResult() {
                ViewName = "_SpotifySearchResultsPartial",
                ViewData = new ViewDataDictionary<List<SpotifySearchResult>>(ViewData, result)
            };
            return partialView;
        }
        public PartialViewResult OnGetGetQueue() {
            string token = "";
            if (User.Identity.IsAuthenticated) {
                token = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            } else {
                token = HttpContext.Session.GetString("Token");
            }
            var result = Spotify.GetQueueItems(token);
            var partialView = new PartialViewResult() {
                ViewName = "_QueuePartial",
                ViewData = new ViewDataDictionary<List<QueueItemResult>>(ViewData, result)
            };
            return partialView;
        }
        public async Task<IActionResult> OnGetAddSong(string id) {
            bool canLike = User.Identity.IsAuthenticated;
            string token = "";
            if (!canLike) {
                token = HttpContext.Session.GetString("Token");
                canLike = Auth.IsAuthorized(token);
            } else {
                token = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            if (canLike) {
                await Spotify.AddToQueue(id, token);
            }
            return new JsonResult(true);

        }

        public IActionResult OnGetNextSong() {
            return new JsonResult(Spotify.QueueNextItem(""));

        }
    }
}