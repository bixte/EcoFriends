using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2;
namespace DataBaseTestLibrary
{
    public class TestDB
    {
        public int AddPost(string title, string subTitle, string description, string postText)
        {
            int postAddedCount = 0;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                postAddedCount = db.Posts.Add(new Posts { Title = title,  SubTitle = subTitle, Desctiption = description, PostText = postText }) !=null ? 1 : 0;
            }
            return postAddedCount;
        }

        public int RemovePost(int Id)
        {
            int postRemoveCount = 0;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                db.Posts.Remove(db.Posts.FirstOrDefault(p => p.Id == Id));
            }
            return postRemoveCount;
        }

        public int CreateUser(string login, string password, string telNumber)
        {
            int CreateUserCount = 0;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                db.Users.Add(new Users() { Login = login, Password = password, TelNmb = telNumber });
            }
            return CreateUserCount;
        }

        public int RemoveUser(string login)
        {
            int RemoveUserCount = 0;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                db.Users.Remove(db.Users.FirstOrDefault(u => u.Login == login));
            }
            return RemoveUserCount;
        }

        public int UpdateUser(int idUser, string login = null, string password = null, string name = null, string phoneNmb = null)
        {
            int UserChangedCount = 0;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                Users user = db.Users.FirstOrDefault(u => u.Id == idUser);
                if (user != null)
                {
                    user.Login = login ?? user.Login;
                    user.Password = password ?? user.Password;
                    user.Name = name ?? user.Name;
                    user.TelNmb = phoneNmb ?? user.TelNmb;
                    UserChangedCount++;
                }
            }
            return UserChangedCount;
        }

    }
}