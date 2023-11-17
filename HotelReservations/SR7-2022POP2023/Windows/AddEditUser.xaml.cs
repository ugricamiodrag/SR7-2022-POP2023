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
using HotelReservations.Model;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for AddEditUser.xaml
    /// </summary>
    public partial class AddEditUser : Window
    {

        private User contextUser;
        private UserService userService;
        public AddEditUser(User? user = null)
        {
            if (user == null)
            {
                contextUser = new User();
            }
            else
            {
                contextUser = user.Clone();
            }
            InitializeComponent();
            userService = new UserService();
            AdjustWindow(user);
            this.DataContext = contextUser;
        }

        private void AdjustWindow(User? user = null)
        {

            UserTypeCB.Items.Add(typeof(Receptionist).Name);
            UserTypeCB.Items.Add(typeof(Administrator).Name);

            if (user != null)
            {
                Title = "Edit user";

                UserTypeCB.SelectedItem = contextUser.UserType;
                UserTypeCB.IsEnabled = false;
            }
            else
            {
                Title = "Add user";
                if (userService.isUsernameTaken(contextUser.Username) == true)
                {
                    MessageBox.Show("Username is already taken.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedUserType = UserTypeCB.SelectedItem as string;
            
            List<string> userProperties = new List<string>
            {
                selectedUserType,
                contextUser.Name,
                contextUser.Surname,
                contextUser.Username,
                contextUser.JMBG
            };

            if (userProperties.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Fill required fields.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            userService.SaveUser(contextUser, selectedUserType);
            this.DialogResult = true;
            this.Close();
            

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult= false;
            this.Close();
        }
    }
}
