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
    /// Interaction logic for RoomTypesWindow.xaml
    /// </summary>
    public partial class RoomTypesWindow : Window
    {

        private ICollectionView view;
        public RoomTypesWindow()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            var roomService = new RoomService();
            var rooms = roomService.GetAllActiveRoomTypes();

            view = CollectionViewSource.GetDefaultView(rooms);
    

            RoomTypesDG.ItemsSource = null;
            RoomTypesDG.ItemsSource = view;
            RoomTypesDG.IsSynchronizedWithCurrentItem = true;

            RoomTypesDG.SelectedIndex = -1;
            view.Refresh();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addRoomTypeWindow = new AddEditRoomType();

            Hide();
            if (addRoomTypeWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoomType = (RoomType)view.CurrentItem;

            if (selectedRoomType != null)
            {
                var editRoomTypeWindow = new AddEditRoomType(selectedRoomType);

                Hide();

                if (editRoomTypeWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("You didn't pick a room.");
                return;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoomType = (RoomType)view.CurrentItem;

            if (selectedRoomType != null)
            {
                var decision = MessageBox.Show($"Do you want to delete the room type - {selectedRoomType.Name}", "Deleting a room", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (decision == MessageBoxResult.Yes)
                {
                    var reservationService = new ReservationService();

                var reservationsWithRoomType = reservationService.GetReservationsByRoomType(selectedRoomType);

                if (reservationsWithRoomType.Any())
                {
                    MessageBox.Show("This room type is associated with reservations and cannot be deleted.");
                    return;
                }

                var priceListService = new PriceListService();
                var pricesForRoomType = priceListService.GetPricesByRoomType(selectedRoomType);

                foreach (var price in pricesForRoomType)
                {
                    price.IsActive = false;
                    priceListService.SavePrice(price);
                }

                selectedRoomType.IsActive = false;

                FillData();
            }
            }
            else
            {
                MessageBox.Show("You didn't pick a room type.");
                return;
            }
        }

        private void RoomTypesDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }
    }
}
