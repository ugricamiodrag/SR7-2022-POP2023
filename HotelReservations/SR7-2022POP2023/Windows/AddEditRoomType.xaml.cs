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
    /// Interaction logic for AddEditRoomType.xaml
    /// </summary>
    public partial class AddEditRoomType : Window
    {

        private RoomService roomService;

        private RoomType contextRoomType;
        public AddEditRoomType(RoomType? roomType = null)
        {
            if (roomType == null)
            {
                contextRoomType = new RoomType();
            }
            else
            {
                contextRoomType = roomType.Clone();
            }

            InitializeComponent();
            roomService = new RoomService();

            AdjustWindow(roomType);

            this.DataContext = contextRoomType;
        }

        public void AdjustWindow(RoomType? roomType = null)
        {
            if (roomType != null)
            {
                Title = "Edit Room Type";
            }
            else
            {
                Title = "Add Room Type";
            }

           
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(contextRoomType.Name))
            {
                MessageBox.Show("Fill required fields.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(CheckDuplicates(contextRoomType.Name) == true)
            {
                MessageBox.Show("Room type already exists.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            roomService.SaveRoomType(contextRoomType);

            DialogResult = true;
            Close();
        }

        private bool CheckDuplicates(string rt)
        {
            var rts = roomService.GetAllActiveRoomTypes();
            foreach (var r in rts)
            {
                if (r.Name.ToLower() == rt.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
