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
    /// Interaction logic for AddEditPriceList.xaml
    /// </summary>
    public partial class AddEditPriceList : Window
    {
        private RoomService roomService = new RoomService();
        private PriceListService plService = new PriceListService();
        Price contextPrice;

        public bool isEditing;

        public AddEditPriceList(Price? price = null)
        {
            
            if (price == null)
            {
                contextPrice = new Price();
                isEditing = false;
            }
            else
            {
                contextPrice = price;
                isEditing = true;
            }


            InitializeComponent();

            AdjustWindow(price);

            this.DataContext = contextPrice;
        }

        private void AdjustWindow(Price? price)
        {
            if (price != null)
            {
                Title = "Edit Price";
            }
            else
            {
                Title = "Add Price";
            }
            var roomTypes = roomService.GetAllActiveRoomTypes();
            RoomTypesCB.ItemsSource = roomTypes;
            var reservationTypes = plService.reservationTypes;
            ReservationTypeCB.ItemsSource = reservationTypes;

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(contextPrice.PriceValue.ToString()) || contextPrice.RoomType == null || contextPrice.ReservationType == null)
            {
                MessageBox.Show("Fill required fields.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (isEditing == false && plService.DoesPriceExistForRoomAndReservationType(contextPrice))
            {
                MessageBox.Show("There is already a price for this type of room and reservation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            plService.SavePrice(contextPrice);

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
