using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        private UserService userService;
        private ICollectionView view;

        public Users()
        {
            userService = new UserService();

            InitializeComponent();
           
            FillData();
            UsersDG.AutoGeneratingColumn += UsersDG_AutoGeneratingColumn;
        }

        // TODO: Korisničke lozinke ne bi trebalo prikazati
        private void FillData()
        {
            var users = userService.GetAllActiveUsers();

            view = CollectionViewSource.GetDefaultView(users);

            view.Filter = DoFilter;

            UsersDG.ItemsSource = null;
            UsersDG.ItemsSource = view;
            UsersDG.IsSynchronizedWithCurrentItem = true;

            

            UsersDG.SelectedIndex = -1;
            view.Refresh();
            
        }

        private bool DoFilter(object userObject)
        {
            var user = userObject as User;

            var usernameSearchParam = UsernameSearchTB.Text;

            if (user.Username.Contains(usernameSearchParam))
            {
                return true;
            }

            return false;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addUsersWindow = new AddEditUser();
            Hide();
            if (addUsersWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = view.CurrentItem as User;

            if (selectedUser != null)
            {
                var editUsersWindow = new AddEditUser(selectedUser);
                Hide();

                if (editUsersWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("You didn't pick an user.");
                return;
            }
        }

        private void UsernameSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var userToDelete = (User)UsersDG.SelectedItem;
            if (userToDelete != null)
            {
                var decision = MessageBox.Show($"Do you want to delete user with the username {userToDelete.Username}", "Deleting an user", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (decision == MessageBoxResult.Yes)
                {
                    userToDelete.IsActive = false;
                    FillData();
                }
                else
                {
                    FillData();
                }

            }
            else
            {
                MessageBox.Show("You didn't pick an user.");
            }
        }

        private void UsersDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            if (e.PropertyName.ToLower() == "password")
            {
                DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
                templateColumn.Header = e.PropertyName;

                ICollectionView collectionView = (ICollectionView)UsersDG.ItemsSource;
                var users = collectionView.Cast<User>();
                int maxLength = users.Max(u => u.Password.Length);

                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
                factory.SetValue(TextBlock.TextProperty, new Binding(e.PropertyName) { Converter = new PasswordConverter(maxLength) });

                DataTemplate cellTemplate = new DataTemplate { VisualTree = factory };
                templateColumn.CellTemplate = cellTemplate;

                e.Column = templateColumn;
            }
        }






    }
}

