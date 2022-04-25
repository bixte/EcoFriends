using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Window
    {
        public Users User { get; set; }
        public int Id { get; set; }


        public ObservableCollection<Comments> Comments
        {
            get
            {
                EcoFriendsEntities db = new EcoFriendsEntities();
                ObservableCollection<Comments> comments = new ObservableCollection<Comments>(db.Comments.Where(comment => comment.IdPost == Id));
                db.Dispose();
                return comments;
            }
        }
        public Post()
        {
            InitializeComponent();
        }
        public void InitPost(int idPost, Users user)
        {
            User = user;
            Id = idPost;
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                DataContext = db.Posts.FirstOrDefault(p => p.Id == idPost);
            };
            CommentsContainer.ItemsSource = Comments;
            if (user.Role == "Admin")
                AdminMenuOpen();
            MyCoins.Text = User.Coins.ToString();
            CommentsCounter.Text = Comments.Count.ToString();

        }
        private void AdminMenuOpen() => PostAdminMenu.Visibility = Visibility.Visible;
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
            Owner = signInWindow;
            Close();
        }

        private void MyNavigation_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                if (menuItem != null)
                {
                    MainWindow blog = new MainWindow();
                    blog.NavClick(menuItem);
                    blog.UserInint(User);
                    blog.WindowState = WindowState;
                    blog.Show();
                    Owner = blog;
                    Close();
                }
            }
        }

        private void BtnLike_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
                btn.Background = new ImageBrush(new BitmapImage(new Uri(@"..\..\..\WpfApp2\img\like.png", UriKind.Relative)));
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                db.Posts.FirstOrDefault(p => p.Id == Id).CounterLike += 1;
                db.SaveChanges();
            }
            MyLike.Text = (int.Parse(MyLike.Text) + 1).ToString();

        }

        private void EditPostText_Click(object sender, RoutedEventArgs e)
        {
            if (PostTextEdit.Visibility == Visibility.Collapsed)
            {
                PostText.Visibility = Visibility.Collapsed;
                PostTextEdit.Visibility = Visibility.Visible;
            }
            else
            {
                PostText.Visibility = Visibility.Visible;
                PostTextEdit.Visibility = Visibility.Collapsed;
            }

        }

        private void PostText_Loaded(object sender, RoutedEventArgs e) => PostTextEdit.AppendText(PostText.Text);

        private void EditPostAccept_Click(object sender, RoutedEventArgs e)
        {
            if (PostText.Visibility == Visibility.Collapsed)
            {
                string text = new TextRange(PostTextEdit.Document.ContentStart, PostTextEdit.Document.ContentEnd).Text;
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    db.Posts.FirstOrDefault(p => p.Id == Id).PostText = text;
                    db.SaveChanges();
                }
                PostText.Text = text;
                PostText.Visibility = Visibility.Visible;
                PostTextEdit.Visibility = Visibility.Collapsed;
            }
        }

        private void CommentsOpen_Click(object sender, RoutedEventArgs e)
        {
            CommentsPopUp.Visibility = Visibility.Visible;
        }

        private void CommentCreate_Click(object sender, RoutedEventArgs e)
        {

            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                db.Comments.Add(new Comments()
                {
                    IdPost = Id,
                    IdUser = User.Id,
                    CommentText = CommentCreateTextBox.Text,
                    DataPublic = $"{DateTime.Now.Day}.{(DateTime.Now.Month < 10 ? 0.ToString() + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())}.{DateTime.Now.Year}",
                    Name = User.Name
                });
                db.SaveChanges();
            }
            CommentsContainer.ItemsSource = Comments;
            SendToPersonNames.Text = string.Empty;
            SendToPerson.Visibility = Visibility.Collapsed;
            MessageBox.Show("Комментарий добавлен");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) => CreatePostName.Text = User.Name;

        private void Button_FeedBack_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {
                if (CommentCreate.Visibility == Visibility.Collapsed)
                    CommentCreate.Visibility = Visibility.Visible;
                
                if (SendToPerson.Visibility == Visibility.Collapsed)
                    SendToPerson.Visibility = Visibility.Visible;
                SendToPersonNames.Text += $"@{btn.DataContext},";
            }
        }

        private void Button_CanselCommentsClick(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button btn)
            {    
                btn.IsEnabled = true;
                CommentsPopUp.Visibility = Visibility.Collapsed;             
            }
        }

        private void Button_CreateCommentCancel_Click(object sender, RoutedEventArgs e) => CommentCreate.Visibility = Visibility.Collapsed;

        private void CreateCommentActive_Click(object sender, RoutedEventArgs e) => CommentCreate.Visibility = Visibility.Visible;
    }
}
