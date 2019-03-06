using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Duckify.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase{

        [HttpGet]
        public bool Test() {
            return true;
        }
    }
}