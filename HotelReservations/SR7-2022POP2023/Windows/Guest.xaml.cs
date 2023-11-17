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
    public partial class Guest : Window
    {
        private ICollectionView view;
        public Guest()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            var guestService = new GuestService();
            var guests = guestService.GetAllActiveGuests();

            view = CollectionViewSource.GetDefaultView(guests);
            view.Filter = DoFilter;

            GuestDG.ItemsSource = null;
            GuestDG.ItemsSource = view;
            GuestDG.IsSynchronizedWithCurrentItem = true;

            GuestDG.SelectedIndex = -1;
            view.Refresh();
        }

        private bool DoFilter(object guestObject)
        {
            var guest = guestObject as Model.Guest;

            var guestSearchParam = GuestSearchTB.Text.ToLower();

            
            if (guest.Name.ToLower().Contains(guestSearchParam) || guest.Surname.ToLower().Contains(guestSearchParam))
            {
                return true;
            }

            return false;
        }

        private void GuestSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void GuestDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addGuestWindow = new AddEditGuest();

            Hide();
            if (addGuestWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {

            var selectedGuest = (Model.Guest)view.CurrentItem;

            if (selectedGuest != null)
            {
                var editGuestWindow = new AddEditGuest(selectedGuest);

                Hide();

                if (editGuestWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("You didn't pick a guest.");
            }
            

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var guestToDelete = (Model.Guest)GuestDG.SelectedItem;
            if (guestToDelete != null)
            {
                guestToDelete.IsActive = false;
                FillData();
            }
            else
            {
                MessageBox.Show("You didn't pick a guest.");
            }
        }
    }
}
