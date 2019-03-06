using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duckify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace Duckify.Pages {
    public class IndexModel : PageModel {

        public void OnGet() {
            
            HttpContext.Session.Set("init", new byte[] { 0x20 });
        }

        public async Task<PartialViewResult> OnGetSearch(string query) {
            var result = await Spotify.SearchTracks(query);
            var partialView = new PartialViewResult() {
                ViewName = "_SpotifySearchResultsPartial",
                ViewData = new ViewDataDictionary<List<SpotifySearchResult>>(ViewData, result)
            };
            return partialView;
        }

        public JsonResult OnGetKey() {
            var auth = new Auth();
            Auth.TempStore.Add(HttpContext.Session.Id, auth);
            return new JsonResult(auth);
        }

        public JsonResult OnGetAuthenticate(string token) {
            //Ask if TempStore doesnt contain session ID by any chance.
            //Used to prevent user from sending their own token without generaing some data server side
            if (!Auth.TempStore.ContainsKey(HttpContext.Session.Id)) {
                return new JsonResult(false);
            }
            //Retrieve token information (key and init vector)
            var auth = Auth.TempStore[HttpContext.Session.Id];
            //TempStore is truly only temporary. We dont need to store this data anymore since client asked for verification of token.
            Auth.TempStore.Remove(HttpContext.Session.Id);
            var ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = auth.ValidateToken(token, ip);
            if (result.validated) {
                //The final token si stored in Session to prevent client from tempering with it.
                HttpContext.Session.SetString("Token", result.token);
                //The only information stored client side about token is that it exists
                Response.Cookies.Append("HasToken", "true");
            }
            return new JsonResult(result.validated);
        }

    }
}
