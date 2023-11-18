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
using HotelReservations.Service;
using HotelReservations.Windows;

namespace HotelReservations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserService userService = new UserService();
        public MainWindow()
        {
            InitializeComponent();
            MainMenu.Visibility = Visibility.Collapsed;
        }

        private void RoomsMI_Click(object sender, RoutedEventArgs e)
        {
            var roomsWindow = new Rooms();
            roomsWindow.Show();
        }


        private void GuestMI_Click(object sender, RoutedEventArgs e)
        {
            var guestWindow = new Guest();
            guestWindow.Show();
        }

        private void UsersMI_Click(object sender, RoutedEventArgs e)
        {
            var usersWindow = new Users();
            usersWindow.Show();
        }

        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            if (AuthenticateLogin(username, password) == true) {
                MainMenu.Visibility = Visibility.Visible;
                LoginForm.Visibility = Visibility.Collapsed;
                var user = userService.ReturnUser(username, password); 
                if (user.UserType.Equals("Receptionist"))
                {
                    AdministratorGeneral.Visibility = Visibility.Collapsed;

                }
                else
                {
                    ReceptionistGeneral.Visibility = Visibility.Collapsed;
                }
                MessageBox.Show("Successful login.", "Verification successful", MessageBoxButton.OK, MessageBoxImage.Information);

                
            }
            else
            {
                ErrorMessageTextBlock.Text = "Wrong login information. Try again.";
                
            }
            
        }

        private bool AuthenticateLogin(string username, string password)
        {
            
            var users = userService.GetAllActiveUsers();
            foreach (var user in users)
            {
                if (user.Username.Equals(username) && user.Password.Equals(password)){
                    return true;
                }
            }
            return false;
        }

        private void ReservationsMI_Click(object sender, RoutedEventArgs e)
        {
            var reservationsWindow = new Reservations();
            reservationsWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logging out", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MainMenu.Visibility = Visibility.Collapsed;
                LoginForm.Visibility = Visibility.Visible;
                UsernameTextBox.Clear();
                PasswordBox.Clear();

            }
        }
    }
}
