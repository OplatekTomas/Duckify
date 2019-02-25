using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duckify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Duckify.Pages {
    public class IndexModel : PageModel {

        public void OnGet() {

        }

        public async Task<PartialViewResult> OnGetSearch(string query) {
            var result = await Spotify.SearchTracks(query);
            var partialView = new PartialViewResult() {
                ViewName = "_SpotifySearchResultsPartial",
                ViewData = new ViewDataDictionary<List<SpotifySearchResult>>(ViewData, result)
            };
            return partialView;
        }
    }
}
