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
using HotelReservations.Windows;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for AddEditReservation.xaml
    /// </summary>
    public partial class AddEditReservation : Window
    {
        private Reservation contextReservation;
        private ReservationService reservationService = new ReservationService();
        private GuestService guestService = new GuestService();
        private PriceListService priceListService = new PriceListService();
        private RoomService roomService = new RoomService();
      

        public AddEditReservation(Reservation? reservation = null)
        {
            if (reservation == null)
            {
                contextReservation = new Reservation();
            }
            else
            {
                contextReservation = reservation.Clone();
            }

            AdjustWindow(reservation);

            InitializeComponent();
        }

        public void AdjustWindow(Reservation reservation)
        {
            if (reservation != null)
            {
                Title = "Edit Reservation";
            }
            else
            {
                Title = "Add Reservation";
            }
        }

        public void SetRoomId(int roomId)
        {
            RoomTB.Text = roomId.ToString();
        }

        public int GetRoomId()
        {
            return int.Parse(RoomTB.Text);
        }

       

        private void btnPickRoom_Click(object sender, RoutedEventArgs e)
        {
            PickARoomWindow pickARoomWindow = new PickARoomWindow();
            pickARoomWindow.RoomIdSetter = SetRoomId;
            pickARoomWindow.ShowDialog();

        }

        private void btnPickGuests_Click(object sender, RoutedEventArgs e)
        {
            PickGuestWindow pickGuestWindow = new PickGuestWindow();

            pickGuestWindow.GuestIdSetter = (selectedGuestIds) =>
            {
                string[] ids = selectedGuestIds.Split('|');
                List<Model.Guest> selectedGuests = new List<Model.Guest>();

                foreach (var id in ids)
                {
                    if (int.TryParse(id, out int guestId))
                    {
                        Model.Guest selectedGuest = guestService.GetGuestById(guestId);
                        if (selectedGuest != null)
                        {
                            selectedGuests.Add(selectedGuest);
                        }
                    }
                }

                dgGuests.ItemsSource = selectedGuests; 
            };

            pickGuestWindow.ShowDialog();
        }

        private void btnStartDate_Click(object sender, RoutedEventArgs e)
        {
            DatePickWindow datePickWindow = new DatePickWindow();

            datePickWindow.DateSelected += (s, selectedDate) =>
            {
                SDateTB.Text = selectedDate.ToShortDateString();
                CalculateTheTotalPrice(SDateTB.Text, EDateTB.Text);
            };

            datePickWindow.ShowDialog();
        }

        private void btnEndDate_Click(object sender, RoutedEventArgs e)
        {
            DatePickWindow datePickWindow = new DatePickWindow();

            datePickWindow.DateSelected += (s, selectedDate) =>
            {
                EDateTB.Text = selectedDate.ToShortDateString();
                CalculateTheTotalPrice(SDateTB.Text, EDateTB.Text);
            };

            datePickWindow.ShowDialog();
        }


        private void CalculateTheTotalPrice(string text1, string text2)
        {
            if (!string.IsNullOrEmpty(text1) && !string.IsNullOrEmpty(text2)) {

                var startDate = DateTime.Parse(text1);
                var endDate = DateTime.Parse(text2);
                var sk = endDate - startDate;
                int difference = (int)sk.Days;
                int id = GetRoomId();
                List<Price> price = new List<Price>();
                if (difference > 1)
                {
                    var roomType = roomService.GetRoomTypeById(id);
                    ReservationType resType = ReservationType.Night;
                    price = GetPricesForRoomType(roomType, resType);
                    ReservationTB.Text = resType.ToString();
                }
                else if (difference > 0)
                {
                    var roomType = roomService.GetRoomTypeById(id);
                    ReservationType resType = ReservationType.Day;
                    price = GetPricesForRoomType(roomType, resType);
                    ReservationTB.Text = resType.ToString();
                }
                else
                {
                    MessageBox.Show("Pick the right dates.");
                }
                double priceValue = 1;

                foreach(var priceElement in price)
                {
                    priceValue = priceElement.PriceValue;
                }

                var totalPrice = priceValue * difference;
                TotalPriceTB.Text = totalPrice.ToString();


            }
        }

        private List<Price> GetPricesForRoomType(RoomType roomType, ReservationType reservationType)
        {

            List<Price> pricesList = priceListService.GetAllPrices();
            var pricesForRoom = pricesList.Where(price =>
                price.RoomType == roomType && price.ReservationType == reservationType && price.IsActive).ToList();

            return pricesForRoom;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int roomId = int.Parse(RoomTB.Text);

            if (Enum.TryParse(ReservationTB.Text, out ReservationType reservationType))
            {
                List<Model.Guest> selectedGuests = (List<Model.Guest>)dgGuests.ItemsSource;
                DateTime startDateTime = DateTime.Parse(SDateTB.Text);
                DateTime endDateTime = DateTime.Parse(EDateTB.Text);
                double totalPrice = double.Parse(TotalPriceTB.Text);
                bool isActive = true;

                Reservation reservation = new Reservation()
                {
                    RoomId = roomId,
                    ReservationType = reservationType,
                    Guests = selectedGuests,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    TotalPrice = totalPrice,
                    IsActive = isActive
                };
                reservationService.SaveReservation(reservation);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid Reservation Type");
            }

            

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void dgGuests_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }
        }
    }
}

