using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    public class IMDbAPIController : Controller
    {
        [HttpGet]
        [Route("TestNotification")]
        public string GetTestNotification()
        {
            return "Get method";
        }
        [HttpDelete]
        [Route("TestNotification")]
        public string DeleteTestNotification()
        {
            return "Delete method";
        }
        [HttpPut]
        [Route("TestNotification")]
        public string PutTestNotification()
        {
            return "Put method";
        }
        [HttpPost]
        [Route("TestNotification")]
        public string PostTestNotification()
        {
            return "Post method";
        }
    }
}
