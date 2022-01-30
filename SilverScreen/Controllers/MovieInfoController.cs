using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    public class MovieInfoController : Controller
    {
        [HttpGet]
        [Route("TestNotification")]
        public string GetTestNotification()
        {
            return "MovieInfo Get method";
        }

        [HttpDelete]
        [Route("TestNotification")]
        public string DeleteTestNotification()
        {
            return "MovieInfo Delete method";
        }

        [HttpPut]
        [Route("TestNotification")]
        public string PutTestNotification()
        {
            return "MovieInfo Put method";
        }

        [HttpPost]
        [Route("TestNotification")]
        public string PostTestNotification()
        {
            return "MovieInfo Post method";
        }
    }
}
