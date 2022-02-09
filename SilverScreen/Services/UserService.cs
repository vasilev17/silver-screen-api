using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SilverScreen.Models;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

        public User AuthenticateUser(Login login)
        {
            User user = null;
            SilverScreenContext context = new SilverScreenContext(configuration);
            AuthenticationService authentication = new AuthenticationService();
            if (context.Users.Where(s => s.Email.Equals(login.Email)).Any())
            {
                if (context.Users.Where(s => s.Password.Equals(authentication.Encrypt(login.Password))).Any())
                {
                    user = context.Users.Where(s => s.Email.Equals(login.Email)).FirstOrDefault();
                }
            }

            return user;
        }

        public string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim("userID", userInfo.Id.ToString())
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
              configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public User RegisterUser(Login login)
        {
            User user = null;
            SilverScreenContext context = new SilverScreenContext(configuration);
            AuthenticationService authentication = new AuthenticationService();
            if (!context.Users.Where(s => s.Email.Equals(login.Email)).Any())
            {
                if (!context.Users.Where(s => s.Username.Equals(login.Username)).Any())
                {


                    User registeredUser = new User()
                    {
                        Username = login.Username,
                        Password = authentication.Encrypt(login.Password),
                        Email = login.Email,
                        Avatar = "sdsadsass",
                        IsAdmin = false,
                        IsDeleted = false,
                        Banned = null
                    };
                    context.Add(registeredUser);
                    context.SaveChanges();
                    user = registeredUser;
                }
            }
            return user;
        }

        public User UploadAvatar(Login login)
        {
            User user = null;
            SilverScreenContext context = new SilverScreenContext(configuration);
            AuthenticationService authentication = new AuthenticationService();
            if (context.Users.Where(s => s.Email.Equals(login.Email)).Any())
            {
                if (context.Users.Where(s => s.Password.Equals(login.Password)).Any())
                {


                    User registeredUser = new User()
                    {
                        Username = user.Username,
                        Password = authentication.Encrypt(user.Password),
                        Email = user.Email,
                        //Avatar = login.Avatar,
                        IsAdmin = false,
                        IsDeleted = false,
                        Banned = null
                    };
                    context.Update(registeredUser);
                    context.SaveChanges();
                    user = registeredUser;
                }
            }
            return user;
        }

    }
}

