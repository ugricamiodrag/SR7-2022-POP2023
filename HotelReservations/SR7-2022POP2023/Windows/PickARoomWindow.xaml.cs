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
    /// Interaction logic for PickARoomWindow.xaml
    /// </summary>
    public partial class PickARoomWindow : Window
    {
        private RoomService roomService = new RoomService();
        public PickARoomWindow()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            List<Room> availableRooms = roomService.GetAllRooms();

            RoomsListBox.ItemsSource = availableRooms;
        }



        private void RoomsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomsListBox.SelectedItem != null)
            {
                RoomDetailsTextBox.Text = RoomsListBox.SelectedItem.ToString();
            }
        }

        public Action<int> RoomIdSetter { get; set; }

        private void btnPickARoom_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsListBox.SelectedItem != null)
            {
                Room selectedRoom = (Room)RoomsListBox.SelectedItem;
                RoomIdSetter.Invoke(selectedRoom.Id);
                this.Close();
            }
        }
    }
}
