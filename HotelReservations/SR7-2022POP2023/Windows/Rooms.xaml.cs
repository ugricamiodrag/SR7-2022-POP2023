using HotelReservations.Service;
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
using HotelReservations.Repository;
using HotelReservations.Model;
using System.ComponentModel;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for Rooms.xaml
    /// </summary>
        public partial class Rooms : Window
        {
            private ICollectionView view;
            public Rooms()
            {
                InitializeComponent();
                FillData();
            }

            public void FillData()
            {
                var roomService = new RoomService();
                var rooms = roomService.GetAllRooms();

                view = CollectionViewSource.GetDefaultView(rooms);
                view.Filter = DoFilter;

                RoomsDG.ItemsSource = null;
                RoomsDG.ItemsSource = view;
                RoomsDG.IsSynchronizedWithCurrentItem = true;

                RoomsDG.SelectedIndex = -1;
                view.Refresh();
        }

            private bool DoFilter(object roomObject)
            {
                var room = roomObject as Room;

                var roomNumberSearchParam = RoomNumberSearchTB.Text;

                if (room.RoomNumber.Contains(roomNumberSearchParam))
                {
                    return true;
                }

                return false;
            }

            private void RoomsDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
            {
                if (e.PropertyName.ToLower() == "IsActive".ToLower())
                {
                    e.Column.Visibility = Visibility.Collapsed;
                }
            }

            private void AddBtn_Click(object sender, RoutedEventArgs e)
            {
                var addRoomWindow = new AddEditRoom();

                Hide();
                if (addRoomWindow.ShowDialog() == true)
                {
                    FillData();
                }
                Show();
            }

            private void EditBtn_Click(object sender, RoutedEventArgs e)
            {
                var selectedRoom = (Room)view.CurrentItem;

                if (selectedRoom != null)
                {
                    var editRoomWindow = new AddEditRoom(selectedRoom);

                    Hide();

                    if (editRoomWindow.ShowDialog() == true)
                    {
                        FillData();
                    }

                    Show();
                }
            }

            private void RoomNumberSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
            {
                view.Refresh();
            }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {

            var roomToDelete = (Room)RoomsDG.SelectedItem;
            if (roomToDelete != null)
            {
                roomToDelete.IsActive = false;
                FillData();
            }
            else
            {
                MessageBox.Show("You didn't pick a room.");
            }
            

        }
    }
}
