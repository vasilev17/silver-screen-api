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
        /// <summary>
        /// Gets the User that corresponds to a perticular ID
        /// </summary>
        /// <param name="userID">The ID, based on which the user is retrieved</param>
        /// <returns>Returns the user object that has the entered ID</returns>
        public User GetUserByID(int userID)
        {
            SilverScreenContext context = new SilverScreenContext();
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



        /// <summary>
        /// Deletes the User that corresponds to a perticular ID
        /// </summary>
        /// <param name="userID">The ID, based on which the user is retrieved</param>
        public void DeleteUserByID(int userID)
        {
            SilverScreenContext context = new SilverScreenContext();
            using (context)
            {
                var user = context.Users.Where(s => s.Id == userID);
                if (user.Any())
                {
                    var userComments = context.Comments.Where(x => x.UserId == userID);
                    if(userComments.Any())
                    {
                        context.RemoveRange(userComments);
                    }

                    var userFriendList = context.FriendLists.Where(x => x.UserId == userID || x.UserId1 == userID);
                    if (userFriendList.Any())
                    {
                        context.RemoveRange(userFriendList);
                    }

                    var userMovieNotifications = context.MovieNotifications.Where(x => x.UserId == userID);
                    if (userMovieNotifications.Any())
                    {
                        context.RemoveRange(userMovieNotifications);
                    }

                    var userMovieRatings = context.MovieRatings.Where(x => x.UserId == userID);
                    if (userMovieRatings.Any())
                    {
                        context.RemoveRange(userMovieRatings);
                    }

                    var userMyList = context.MyLists.Where(x => x.UserId == userID);
                    if (userMyList.Any())
                    {
                        context.RemoveRange(userMyList);
                    }

                    var userNotifications = context.Notifications.Where(x => x.UserId == userID || x.AuthorId == userID);
                    if (userNotifications.Any())
                    {
                        context.RemoveRange(userNotifications);
                    }

                    context.Remove(user.FirstOrDefault());
                    context.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Validates the user credentials
        /// </summary>
        /// <param name="login">Object from the Login class. It contains (Email, Password, Username)</param>
        /// <returns>Returns a list containing all the information about the specific user</returns>
        public User AuthenticateUser(Login login)
        {
            User user = null;
            SilverScreenContext context = new SilverScreenContext();
            AuthenticationService authentication = new AuthenticationService();
            if (context.Users.Where(s => s.Email.Equals(login.Email)).Any())
            {
                if (context.Users.Where(s => s.Password.Equals(authentication.Encrypt(login.Password))).Any())
                {
                    user = context.Users.Where(s => s.Email.Equals(login.Email)).FirstOrDefault();
                }
                else
                {
                    throw new Exception("Wrong Password!");
                }
            }
            else
            {
                throw new Exception("This Email doesn't exist");
            }

            return user;
        }
        /// <summary>
        /// Creates a new Token which contains the userID and the Username
        /// </summary>
        /// <param name="userInfo">Object from the User class. It contains (Email, Password, Username, etc...)</param>
        /// <returns>Returns a Token containing your information</returns>
        public string GenerateJSONWebToken(User userInfo, bool rememberMe)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SSJWTKey")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim("userID", userInfo.Id.ToString())
            };
            var time = DateTime.UtcNow;
            if (rememberMe)
            {
                time = time.AddMonths(1);
            }
            else
            {
                time = time.AddDays(1);
            }
            var token = new JwtSecurityToken("silverscreenbg",
              "silverscreenbg",
              claims,
              expires: time,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Creates a new user, requires your (Email, Password and Username)
        /// </summary>
        /// <param name="login">Object from the Login class. It contains (Email, Password, Username)</param>
        /// <returns>Returns a list containing all the information about the specific user</returns>
        public User RegisterUser(Login login)
        {
            User user = null;
            var confirmPassword = " ";
            SilverScreenContext context = new SilverScreenContext();
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
                                if (login.confirmPassword.Equals(login.Password))
                                {


                                    User registeredUser = new User()
                                    {
                                        Username = login.Username,
                                        Password = authentication.Encrypt(login.Password),
                                        Email = login.Email,
                                        Avatar = Environment.GetEnvironmentVariable("PROD_FRONTEND") != null ? Environment.GetEnvironmentVariable("PROD_FRONTEND") + "/DefaultProfilePic.png" : "http://localhost:3000/DefaultProfilePic.png",
                                        IsAdmin = false,
                                        IsDeleted = false,
                                        Banned = null
                                    };
                                    confirmPassword = login.confirmPassword;

                                    context.Add(registeredUser);
                                    context.SaveChanges();
                                    user = registeredUser;
                                }else
                                {
                                    throw new Exception("Passwords do not match");
                                }
                            }
                            else
                            {
                                throw new Exception("The password should not contain white spaces");
                            }
                        }
                        else
                        {
                            throw new Exception("The password requires at least one upper case letter");
                        }
                    }
                    else
                    {
                        throw new Exception("The password requires more than 6 and less than 25 characters");
                    }
                }
                else
                {
                    throw new Exception("This username is already in use");
                }

            }
            else
            {
                throw new Exception("This email is already in use");
            }

            return user;
        }

        public User UploadAvatar(Login login)
        {
            User user = null;
            SilverScreenContext context = new SilverScreenContext();
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

        /// <summary>
        /// Creates a friend system between two users if they are not friends already.
        /// </summary>
        /// <param name="userID">The ID of the user you are logged into</param>
        /// <param name="friendID">The ID of the user you are friends with</param>
        /// <returns>Returns if they are friends now or they were already</returns>
        public int AddFriend(int userID, int friendID)
        {
            SilverScreenContext context = new SilverScreenContext();
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

