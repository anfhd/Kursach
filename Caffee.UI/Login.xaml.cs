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

namespace Caffee.UI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public bool Success { get; set; }
        public Role Role { get; set; }
        public Login()
        {
            InitializeComponent();
        }

        private Dictionary<Role, KeyValuePair<string, string>> accounts = new()
        {
            {Role.Visitor, KeyValuePair.Create("visitor", "1111") },
            {Role.Employee, KeyValuePair.Create("employee", "1111") }
        };

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            var login = this.LoginTextBox.Text;
            var password = this.PasswordTextBox.Text;

            foreach(var account in accounts)
            {
                if(account.Value.Key == login)
                {
                    if(account.Value.Value == password)
                    {
                        Success = true;
                        this.Role = account.Key;

                        this.Close();
                        return;
                    }
                }
            }

            MessageBox.Show("Invalid login or password", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
            this.Close();
            return;
        }
    }
}
