using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SliverScreen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationManagementController : Controller
    {
        [HttpGet]
        [Route("TestNotification")]
        public ActionResult<string> TestNotification()
        {
            return "Hello world!";
        }
    }
}
