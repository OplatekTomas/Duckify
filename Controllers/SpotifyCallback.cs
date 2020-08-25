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
        private IConfiguration Configuration;

        private SpotifyService _service;
        public SpotifyCallback(IConfiguration configuration, SpotifyService service) {
            Configuration = configuration;
            _service = service;

        }

        [Route("/spotify-callback")]
        public async Task<IActionResult> Route() {
            if (Request.Query.ContainsKey("error") || !Request.Query.ContainsKey("code")) {
                var returnMessage = "Error has occured while trying to get authorization:\n";
                returnMessage += Request.Query["state"];
                return Content(returnMessage);
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Helper.Base64Encode(Configuration["SpotifyID"] + ":" + Configuration["SpotifySecret"]));

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("code", Request.Query["code"]),
                new KeyValuePair<string, string>("redirect_uri", "https://localhost:5001/spotify-callback"),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            var response = client.PostAsync("https://accounts.spotify.com/api/token", formContent).Result;

            var responseContent = response.Content;
            var responseString = responseContent.ReadAsStringAsync().Result;
            
            return Content("");
        }

        
    }
}