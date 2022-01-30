using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("UserManagement")]
        public string GetTestUser()
        {
            return "Get method";
        }
        [HttpDelete]
        [Route("UserManagement")]
        public string DeleteTestUser()
        {
            return "Delete method";
        }
        [HttpPost]
        [Route("UserManagement")]
        public string PostTestUser()
        {
            return "Post method";
        }
        [HttpPut]
        [Route("UserManagement")]
        public string PutTestUser()
        {
            return "Put method";
        }
    }
}
