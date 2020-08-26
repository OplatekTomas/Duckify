using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Duckify.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Duckify.Controllers {
    public class SpotifyCallback : Controller {

        private SpotifyService _service;
        public SpotifyCallback(SpotifyService service) {
            _service = service;

        }

        [Route("/spotify-callback")]
        public async Task<IActionResult> Route() {
            if (Request.Query.ContainsKey("error") || !Request.Query.ContainsKey("code")) {
                var returnMessage = "Error has occured while trying to get authorization:\n";
                returnMessage += Request.Query["state"];
                return Content(returnMessage);
            }

            var response = await _service.Authenticate(Request.Query["code"]);
            if (response == null) {
                return Redirect("/dashboard");
            }
            var ret = "Error has occured while trying to get authorization:\n";
            ret += response;
            return Content(ret);
        }

        
    }
}