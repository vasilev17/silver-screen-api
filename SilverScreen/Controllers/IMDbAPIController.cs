using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IMDbAPIController : Controller
    {
        private IConfiguration configuration;

        public IMDbAPIController(IConfiguration config)
        {
            configuration = config;
        }
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
        [Route("AddMovieToDB")]
        public void AddMovieToDB(string title)
        {
            try
            {
                IMDbAPIService iMDbAPIService = new IMDbAPIService(configuration);
                iMDbAPIService.LoadMovieIntoDB(title);
            }
            catch (Exception)
            {

            }
        }
    }
}
