using HotelReservations.Model;
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

namespace HotelReservations.Windows
{
    public partial class AddEditRoom : Window
    {
        private RoomService roomService;
        private Room roomToEdit;

        public AddEditRoom(Room room = null)
        {
            InitializeComponent();
            roomService = new RoomService();
            roomToEdit = room;

            AdjustWindow(room);
        }

        public void AdjustWindow(Room room = null)
        {
            if (room != null)
            {
                Title = "Edit Room";
                RoomNumberTB.Text = room.RoomNumber;
                HasTVCB.IsChecked = room.HasTV;
                HasMiniBarCB.IsChecked = room.HasMiniBar;

                if (room.RoomType != null)
                {
                    RoomTypesCB.SelectedItem = room.RoomType.Name;
                }
            }
            else
            {
                Title = "Add Room";
            }

            var roomTypes = roomService.GetAllRoomTypes();
            foreach (var roomType in roomTypes)
            {
                RoomTypesCB.Items.Add(roomType.Name);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (roomToEdit != null)
            {
                roomToEdit.RoomNumber = RoomNumberTB.Text;
                roomToEdit.HasTV = HasTVCB.IsChecked ?? false;
                roomToEdit.HasMiniBar = HasMiniBarCB.IsChecked ?? false;
                var selectedRoomTypeName = (string)RoomTypesCB.SelectedItem;

                if (selectedRoomTypeName != null)
                {
                    var selectedRoomType = roomService.GetRoomTypeByName(selectedRoomTypeName);
                    roomToEdit.RoomType = selectedRoomType;
                }

                roomService.UpdateRoom(roomToEdit);
            }
            else
            {
                var newRoom = new Room();
                newRoom.Id = roomService.GetNextIdValue();
                newRoom.RoomNumber = RoomNumberTB.Text;
                newRoom.HasTV = HasTVCB.IsChecked ?? false;
                newRoom.HasMiniBar = HasMiniBarCB.IsChecked ?? false;
                newRoom.IsActive = true;

                var selectedRoomTypeName = (string)RoomTypesCB.SelectedItem;
                if (selectedRoomTypeName != null)
                {
                    var selectedRoomType = roomService.GetRoomTypeByName(selectedRoomTypeName);
                    newRoom.RoomType = selectedRoomType;
                }

                roomService.SaveRoom(newRoom);
            }

            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}