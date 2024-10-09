using Caffee.Models;
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
    /// Interaction logic for PersonInfo.xaml
    /// </summary>
    public partial class PersonInfo : Window
    {
        public PersonInfo()
        {
            InitializeComponent();
        }

        public PersonInfo(Person person, Role role) : this()
        {
            InitializeComponent();

            this.FullNameLabel.Content = person.FirstName + " " + person.LastName;
            this.BirthDateLabel.Content = person.BirthDate.ToString();

            this.RoleLabel.Content = role.ToString();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
