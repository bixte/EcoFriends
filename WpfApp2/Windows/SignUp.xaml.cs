using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }
        private void SignUpClick_Click(object sender, RoutedEventArgs e)
        {
            using (EcoFriendsEntities db = new EcoFriendsEntities())
            {
                if (!db.Users.Any(u => u.Login == LogBox.Text))
                {
                    db.Users.Add(new Users
                    {
                        Login = LogBox.Text,
                        Password = PasBox.Text,
                        TelNmb = TelNmbBox.Text,
                        Role = "User",
                        Name = $"Guest"
                    });
                    db.SaveChanges();
                }
            }
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
            Close();
        }

        private void PasBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Source is TextBox textBox)
            {
                if (textBox.GetLineLength(0) <= 3)
                {
                    PasBoxStatus.Foreground = new SolidColorBrush(Colors.Salmon);
                    PasBoxStatus.Text = "Слишком короткий пароль";
                }
                if (textBox.GetLineLength(0) >= 6)
                {
                    PasBoxStatus.Foreground = new SolidColorBrush(Colors.Goldenrod);
                    PasBoxStatus.Text = "Средний уровень пароля";
                }
                if (textBox.GetLineLength(0) >= 10)
                {
                    PasBoxStatus.Foreground = new SolidColorBrush(Colors.Green);
                    PasBoxStatus.Text = "Высокий уровень пароля";
                }
                if (textBox.GetLineLength(0) >= 15)
                    PasBoxStatus.Text = "Потрясный пароль";

                if (textBox.GetLineLength(0) >= 20)
                    PasBoxStatus.Text = "А вы точно запомнили пароль?";
            }
        }
        private async void LogBox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool existLogin = false;
            string login = LogBox.Text;
            if (login != string.Empty)
                existLogin = await Task.Run(() =>
                {
                     using (EcoFriendsEntities db = new EcoFriendsEntities())
                     {
                         return db.Users.Any(u => u.Login == login);
                     }
                });
            if (existLogin)
            {
                LoginStatus.Foreground = new SolidColorBrush(Colors.Salmon);
                LoginStatus.Text = "Логин занят";
            }
            else
            {
                LoginStatus.Foreground = new SolidColorBrush(Colors.Green);
                LoginStatus.Text = "Логин свободен";
            }
        }

        private void BackToSignIn_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow signInWindow = new SignInWindow();
            signInWindow.Show();
            Owner = signInWindow;
            Close();
        }
    }
}
