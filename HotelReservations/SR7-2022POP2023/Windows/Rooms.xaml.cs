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

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        public Rooms()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            var roomService = new RoomService();
            var rooms = roomService.GetAllRooms();

            RoomsDG.ItemsSource = null;
            RoomsDG.ItemsSource = rooms;
        }

        private void RoomsDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addRoomWindow = new AddEditRoom();

            Hide();
            if(addRoomWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsDG.SelectedItem == null)
            {
                MessageBox.Show("Please select a room to edit.", "No room selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Model.Room selectedRoom = (Model.Room)RoomsDG.SelectedItem;

            var editRoomWindow = new AddEditRoom(selectedRoom);

            Hide();
            if (editRoomWindow.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }


        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            RoomService roomService = new RoomService();
            var result = MessageBox.Show("Deleting the room", "Do you want to delete the room? ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes) {
                Model.Room roomToDelete = (Model.Room)RoomsDG.SelectedItem;
                var pickedRoom = roomService.GetAllRooms().Find(rn => rn.Id == roomToDelete.Id);
                pickedRoom.IsActive = false;

            }

            Show();
            Close();



        }
    }
}
