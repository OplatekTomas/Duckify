using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Duckify.API {
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase {

        public SignInManager<IdentityUser> SignInManager { get; set; }

        public SpotifyController(SignInManager<IdentityUser> signInManager) {
            this.SignInManager = signInManager;
        }


        [HttpGet("ip", Name = "GetIp")]
        public string GetIP() {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        [HttpGet("currentSong", Name = "GetCurrentSong")]
        public JsonResult GetCurrentSong() {
            return new JsonResult(Spotify.GetCurrentItem(""));
        }

        [HttpGet("qHash", Name = "GetQueueHash")]
        public JsonResult GetQueueHash() {
            return new JsonResult(Spotify.Hash);
        }

        //These are experimantal features that are not fully tested or implemented. They may or may not be used in a future release
        /*
        [HttpGet("upvote/{id}", Name = "LikeSong")]
        public async Task<IActionResult> Upvote(string id) {
            var isAuth = await IsAuthorized();
            if (!isAuth.success) {
                return Unauthorized();
            }
            var result = await Spotify.AddToQueue(id, isAuth.token);
            return Ok(true);
        }

        private async Task<(bool success, string token)> IsAuthorized() {
            var headers = HttpContext.Request.Headers;
            if (!headers.ContainsKey("Authorization")) {
                return (false, null);
            }
            var token = headers["Authorization"][0].Replace(' ', '+');
            var regexString = "(?<=Bearer\\+)((?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)|(([a-zA-Z0-9]{8}-([a-zA-Z0-9]{4}-){3})[a-zA-Z0-9]{12}))?$";
            var match = Regex.Match(token, regexString);
            if (!match.Success) {
                return (false, null);
            }
            token = match.Value;
            var user = await SignInManager.UserManager.FindByIdAsync(token);
            if (!(Auth.ValidUsers.ContainsKey(token) || user != null)) {
                return (false, null);
            }
            return (true, token);
        }*/

    }
}