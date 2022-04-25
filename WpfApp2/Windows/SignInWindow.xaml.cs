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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Configuration;
namespace WpfApp2
{
    public partial class SignInWindow : Window
    {
        public SignInWindow() => InitializeComponent();

        private void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LogBox.Text != string.Empty && PasBox.Password != string.Empty)
            {
                using (EcoFriendsEntities db = new EcoFriendsEntities())
                {
                    Users user = db.Users?.FirstOrDefault(u => u.Login.ToUpper() == LogBox.Text.ToUpper() && u.Password.ToUpper() == PasBox.Password.ToUpper());
                    if (user != null)
                    {
                        TransToNextWindow(user);
                        return;
                    }
                }
                MessageBox.Show("не найден", "Авторизация");
            }
        }

        private void TransToNextWindow(Users user)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.UserInint(user);
            mainWindow.Show();
            Owner = mainWindow;
            Close();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp = new SignUp();
            signUp.Show();
            Owner = signUp;
            Close();
        }
    }
}
