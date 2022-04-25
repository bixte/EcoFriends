using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace WpfApp2
{
    public class DataProv
    {
        public List<Posts> Posts
        {
            get
            {
                List<Posts> posts;
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    posts = db.Posts.ToList();
                }
                return posts;
            }
        }
        public List<EventsPost> Events
        {
            get
            {
                List<EventsPost> events;
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    events =  db.EventsPost.ToList();
                }
                return events;
            }
        }
        public Comments[] Comments
        {
            get
            {
                Comments[] comments = Array.Empty<Comments>();
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    comments = db.Comments.ToArray();
                }
                return comments;
            }
        }
    }

}
