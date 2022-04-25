using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public Users User { get; set; }
        public ObservableCollection<EventsPost> UserTasks { get; set; } = new ObservableCollection<EventsPost>();
        private readonly List<Grid> Grids;
        public MainWindow()
        {
            InitializeComponent();
            Grids = new List<Grid> { MyBlog, MyProfile, MyEvent, MyTasks };
        }

        public void UserInint(Users user)
        {
            User = user;
            MyProfile.DataContext = User;
            if (user.Role == "Admin")
                AdminPrivileges();
            MyCoins.Text = User.Coins.ToString();

            void AdminPrivileges()
            {
                AdminPanelPosts.Visibility = Visibility.Visible;
                AdminPanelEvents.Visibility = Visibility.Visible;
            }
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
            Owner = signInWindow;
            Close();
        }

        /*MENU*/
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
                if (menuItem.Header != null)
                    NavClick(menuItem);
        }

        public void NavClick(MenuItem menuItem)
        {
            foreach (Grid item in Grids)
                item.Visibility = Visibility.Collapsed;

            if (menuItem.Header.ToString() == "My Profile")
                MyProfile.Visibility = Visibility.Visible;

            if (menuItem.Header.ToString() == "Blog")
                MyBlog.Visibility = Visibility.Visible;

            if (menuItem.Header.ToString() == "Events")
                MyEvent.Visibility = Visibility.Visible;

            if (menuItem.Header.ToString() == "Tasks")
                MyTasks.Visibility = Visibility.Visible;

        }


        /* EVENTS */

        private void CreateEvent_Click(object sender, RoutedEventArgs e) => PopUpCreateEvent.Visibility = Visibility.Visible;

        private void CreateEventCancel_Click(object sender, RoutedEventArgs e) => PopUpCreateEvent.Visibility = Visibility.Collapsed;

        private void CreateEventBtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (CreateEventTitle.Text != string.Empty && CreateEventCoin.Text != string.Empty && CreateEventDescription.Text != string.Empty)
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    db.EventsPost.Add(
                        new EventsPost
                        {
                            Title = CreateEventTitle.Text,
                            Description = CreateEventDescription.Text,
                            Coins = int.Parse(CreateEventCoin.Text)
                        });
                    db.SaveChanges();
                    MyEvents.ItemsSource = db.EventsPost.ToList();
                    PopUpCreatePost.Visibility = Visibility.Collapsed;
                }
        }
        private void PopUpTo_Click(object sender, RoutedEventArgs e)
        {
            MyPopUp.Visibility = Visibility.Visible;
            MyEvent.Background = new SolidColorBrush(Color.FromRgb(60, 59, 70));
            int postID = (int)(e.Source as Button).DataContext;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                if (db.UsersToEvent.FirstOrDefault(uEvent => uEvent.IdEvent == postID && uEvent.IdUser == User.Id) == null)
                {
                    db.UsersToEvent
                    .Add(
                        new UsersToEvent
                        {
                            IdEvent = postID,
                            IdUser = User.Id
                        });
                    db.SaveChanges();
                    UserTasks.Add(
                        db.EventsPost.FirstOrDefault(ePost => ePost.Id == postID));
                }
                else
                    MessageBox.Show("вы уже участвуете");
            }
        }

        private void PopUpClose_Click(object sender, RoutedEventArgs e)
        {
            MyPopUp.Visibility = Visibility.Collapsed;
            MyEventContainer.Visibility = Visibility.Visible;
            MyEvent.Background = default;
        }
        private void DeleteEventBtn_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    db.EventsPost.Remove(db.EventsPost.FirstOrDefault(eventPost => eventPost.Id == (int)btn.DataContext));
                    db.SaveChanges();
                    MyEvents.ItemsSource = db.EventsPost.ToList();
                }
        }

        private void DeleteEventCard_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                PopUpDeletPost.Visibility = Visibility.Visible;
                PopUpDeletPost.DataContext = new DeleteArg("Event", (int)btn.DataContext);
            }
        }


        /*PROFILE*/

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (MyLogin.Text != string.Empty && MyPassword.Text != string.Empty)
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    Users user = db.Users.FirstOrDefault(u => u.Id == User.Id);
                    user.Name = MyName.Text;
                    user.Login = MyLogin.Text;
                    int count = db.SaveChanges();
                    MessageBox.Show($"Изменения внесены", "Профиль");
                }
            else
                MessageBox.Show("пустные поля недопущены");

        }

        /* POSTS*/

        private void PostTo_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                Post post = new Post
                {
                    WindowState = WindowState
                };
                post.InitPost((int)btn.DataContext, User);
                post.Show();
                Owner = post;
                Close();
            }
        }
        private void CreatePost_Click(object sender, RoutedEventArgs e) => PopUpCreatePost.Visibility = Visibility.Visible;

        private void CreatePostCancel_Click(object sender, RoutedEventArgs e) => PopUpCreatePost.Visibility = Visibility.Collapsed;

        private async void CreatePostBtnCreate_Click(object sender, RoutedEventArgs e)
        {

            if (CreatePostTitle.Text != string.Empty && CreatePostDescription.Text != string.Empty)
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    db.Posts.Add(new Posts
                    {
                        Title = CreatePostTitle.Text,
                        SubTitle = CreatePostSubTitle.Text,
                        Desctiption = CreatePostDescription.Text,
                        PostText = new TextRange(
                            CreatePostText.Document.ContentStart,
                            CreatePostText.Document.ContentEnd).Text
                    });
                    await db.SaveChangesAsync();
                    MyPosts.ItemsSource = db.Posts.ToList();
                    PopUpCreatePost.Visibility = Visibility.Collapsed;
                }

        }
        private void DeletePost_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                PopUpDeletPost.Visibility = Visibility.Visible;
                PopUpDeletPost.DataContext = new DeleteArg("Post", (int)btn.DataContext);
            }

        }

        private void PopUpSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                if (btn.Content.ToString() == "Да")
                {
                    DeleteArg deletearg = btn.DataContext as DeleteArg;
                    EcoFriendsEntities db = new EcoFriendsEntities();
                    if (deletearg.Element == "Post")
                    {
                        db.Posts.Remove(db.Posts.FirstOrDefault(p => p.Id == deletearg.Id));
                        db.SaveChanges();
                        MyPosts.ItemsSource = db.Posts.ToList();
                    }

                    if (deletearg.Element == "Event")
                    {
                        var eventPost = db.EventsPost.FirstOrDefault(p => p.Id == deletearg.Id);
                        MyEvents.ItemsSource = db.EventsPost.ToList();
                        if (deletearg.IsComleted)
                        {
                            var usersId = db.UsersToEvent.Where(uEvent => uEvent.IdEvent == eventPost.Id).Select(uEvent => uEvent.IdUser).ToList();
                            foreach (var item in usersId)
                            {
                                db.Users.First(u => u.Id == item).Coins += eventPost.Coins;
                            }
                            db.EventsPost.Remove(eventPost);
                            db.SaveChanges();
                            MyEvents.ItemsSource = db.EventsPost.ToList();
                        }
                    }
                    db.Dispose();
                    PopUpDeletPost.Visibility = Visibility.Collapsed;
                }
                if (btn.Content.ToString() == "Нет")
                    PopUpDeletPost.Visibility = Visibility.Collapsed;
            }
        }

        /*TASKS*/

        private void MyTasks_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> eventsSelectIdPost;
            EcoFriendsEntities db = new EcoFriendsEntities();
            eventsSelectIdPost = db.UsersToEvent.Where(eventUser => eventUser.IdUser == User.Id).Select(eUser => eUser.IdEvent).AsQueryable().ToList();
            db.Dispose();
            if (eventsSelectIdPost != null)
            {
                foreach (var id in eventsSelectIdPost)
                    foreach (var item in new DataProv().Events)
                        if (item.Id == id)
                            UserTasks.Add(item);
            }
            MyTasksCollection.ItemsSource = UserTasks;
        }

        /*BTNS LOADED*/

        private void DeletePostBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.Role == "Admin")
                (sender as Button).Visibility = Visibility.Visible;
        }

        private void DeletePostEventBtn_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (User.Role == "Admin")
                (sender as Button).Visibility = Visibility.Visible;
        }

        private void DeleteEventCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.Role == "Admin")
                (sender as Button).Visibility = Visibility.Visible;
        }

        private void CreateEventCoin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void AcceptEventCard_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                PopUpDeletPost.Visibility = Visibility.Visible;
                PopUpDeletPost.DataContext = new DeleteArg("Event", Id: (int)btn.DataContext, isComleted: true);
            }
        }
    }

    public class DeleteArg
    {
        public bool IsComleted { get; }
        public DeleteArg(string element, int Id, bool isComleted = false)
        {
            Element = element;
            this.Id = Id;
            IsComleted = isComleted;
        }
        public string Element { get; }
        public int Id { get; }
    }

}
