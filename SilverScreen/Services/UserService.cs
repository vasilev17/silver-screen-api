using Microsoft.Extensions.Configuration;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Services
{
    public class UserService
    {
        private IConfiguration configuration;

        public UserService(IConfiguration config)
        {
            configuration = config;
        }


        public User GetUserByID(int userID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            using (context)
            {
                var user = context.Users.Where(s => s.Id == userID);
                if (user.Any())
                {
                    return user.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

