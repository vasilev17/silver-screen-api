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
                else
                {
                    throw new Exception("Wrong Password");
                }
            }
            else
            {
                throw new Exception("This Email doesn't exist");
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
                    if (login.Password.Length >= 6 && login.Password.Length <= 25)
                    {
                        if (login.Password.Any(char.IsUpper))
                        {
                            if (!login.Password.Contains(" "))
                            {
                                User registeredUser = new User()
                                {
                                    Username = login.Username,
                                    Password = authentication.Encrypt(login.Password),
                                    Email = login.Email,
                                    Avatar = "https://i.ibb.co/zVd6Vnv/defautprifilepic.png",
                                    IsAdmin = false,
                                    IsDeleted = false,
                                    Banned = null
                                };
                                context.Add(registeredUser);
                                context.SaveChanges();
                                user = registeredUser;
                            }
                            else
                            {
                                throw new Exception("No white space!");
                            }
                        }
                        else
                        {
                            throw new Exception("At least one upper case");
                        }
                    }
                    else
                    {
                        throw new Exception("The password requires more than 6 and less than 25 characters");
                    }
                }
                else
                {
                    throw new Exception("This username is already used");
                }

            }
            else
            {
                throw new Exception("This Email is already used");
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


        public int AddFriend(int userID, int friendID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            if (!context.FriendLists.Where(x => (x.UserId == userID && x.UserId1 == friendID) || (x.UserId == friendID && x.UserId1 == userID)).Any())
            {
                FriendList frList1 = new FriendList
                {
                    UserId = userID,
                    UserId1 = friendID
                };
                FriendList frList2 = new FriendList
                {
                    UserId = friendID,
                    UserId1 = userID
                };
                context.Add(frList1);
                context.Add(frList2);
                context.SaveChanges();
                return 0;
            }
            return -1;
        }
    }
}

