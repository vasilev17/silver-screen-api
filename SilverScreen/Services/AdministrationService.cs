using SilverScreen.Models.Tables;

namespace SilverScreen.Services
{
    public class AdministrationService
    {
        public bool isUserAdministrator(int userId)
        {
            var context = new SilverScreenContext();
            var user = context.Users.Find(userId);
            if (user.IsAdmin && !(user.IsDeleted || user.Banned > System.DateTime.UtcNow))
            {
                return true;
            }
            return false;
        }
    }
}
