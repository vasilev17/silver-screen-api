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
        /// <summary>
        /// A GET request that calls the "LoadMovieIntoDB" method from the "IMDbAPIService" service in order to save a movie into the database based on its title
        /// </summary>
        /// <param name="title">The method uses this string to send a get request for the particular movie we want to add to the database</param>
        [HttpPost]
        [Route("AddMovieToDB")]
        public void AddMovieToDB(string title)
        {
            try
            {
                IMDbAPIService iMDbAPIService = new IMDbAPIService(configuration);
                iMDbAPIService.LoadMovieIntoDB(title);
            }
            catch(MySql.Data.MySqlClient.MySqlException)
            {

            }
        }
    }
}
