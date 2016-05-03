using BggApi.Models;

namespace BggUwp.Data.Models
{
    public class UserDataItem : User
    {
        public UserDataItem(User user)
        {
            BggUserId = user.BggUserId;
            Username = user.Username;
            Avatar = user.Avatar;
            FirstName = user.FirstName;
            LastName = user.LastName;
            YearRegistered = user.YearRegistered;
        }
    }
}
