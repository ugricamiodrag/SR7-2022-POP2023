using System;
using System.Collections.Generic;
using System.Globalization;
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
            InitializeComponent();

            if (reservation == null)
            {
                AdjustWindow(reservation);
                contextReservation = new Reservation();

            }
            else
            {
                contextReservation = reservation.Clone();
                PopulateFieldsFromReservation(contextReservation);
                AdjustWindow(reservation);
            }
            


            this.DataContext = contextReservation;
        }

        



        private void PopulateFieldsFromReservation(Reservation reservation)
        {
            if (reservation != null)
            {
                if (reservation.RoomId != null)
                {
                    RoomTB.Text = reservation.RoomId.ToString();
                }
                else
                {
                   
                    MessageBox.Show("RoomId is null in the reservation data.");
                    RoomTB.Text = string.Empty;
                }
                ReservationTB.Text = reservation.ReservationType.ToString();
                dgGuests.ItemsSource = reservation.Guests;
                SDateTB.Text = reservation.StartDateTime.ToString("dd.MM.yyyy. HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
              
            }
            else
            {
         
                MessageBox.Show("Reservation data is empty.");
            }
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
                SDateTB.Text = selectedDate.ToString("dd.MM.yyyy. HH:mm:ss", new System.Globalization.CultureInfo("en-US"));
                
            };

            datePickWindow.ShowDialog();
        }

       


        //private void CalculateTheTotalPrice(string text1, string text2)
        //{
        //    if (!string.IsNullOrEmpty(text1) && !string.IsNullOrEmpty(text2))
        //    {
               

        //        string[] dateFormats = { "dd.MM.yyyy. HH:mm:ss", "d.MM.yyyy. HH:mm:ss" };

        //        DateTime startDate, endDate;

        //        if (DateTime.TryParseExact(text1, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
        //            DateTime.TryParseExact(text2, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        //        {
        //            var sk = endDate - startDate;
        //            int difference = (int)sk.TotalDays;
        //            int id = GetRoomId();
        //            List<Price> price = new List<Price>();

        //            if (difference > 1)
        //            {
        //                var roomType = roomService.GetRoomTypeById(id);
        //                ReservationType resType = ReservationType.Night;
        //                price = GetPricesForRoomType(roomType, resType);
        //                ReservationTB.Text = resType.ToString();
        //            }
        //            else if (difference > 0)
        //            {
        //                var roomType = roomService.GetRoomTypeById(id);
        //                ReservationType resType = ReservationType.Day;
        //                price = GetPricesForRoomType(roomType, resType);
        //                ReservationTB.Text = resType.ToString();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Pick the right dates.");
        //            }

        //            double priceValue = 1;

        //            foreach (var priceElement in price)
        //            {
        //                priceValue = priceElement.PriceValue;
        //            }

        //            var totalPrice = priceValue * difference;
        //            TotalPriceTB.Text = totalPrice.ToString();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Invalid date format. Please provide dates in the correct format.");
        //        }
        //    }
        //}


        private List<Price> GetPricesForRoomType(RoomType roomType, ReservationType reservationType)
        {

            List<Price> pricesList = priceListService.GetAllPrices();
            var pricesForRoom = pricesList.Where(price =>
                price.RoomType.Name == roomType.Name && price.ReservationType == reservationType && price.IsActive == true).ToList();

            return pricesForRoom;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RoomTB.Text) &&
                !string.IsNullOrEmpty(ReservationTB.Text) &&
                dgGuests.ItemsSource != null &&
                !string.IsNullOrEmpty(SDateTB.Text))
            {
                int roomId = int.Parse(RoomTB.Text);
                if (Enum.TryParse(ReservationTB.Text, out ReservationType reservationType))
                {
                    string dateFormat = "dd.MM.yyyy. HH:mm:ss";

                    if (DateTime.TryParseExact(SDateTB.Text, dateFormat, new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out DateTime startDateTime))
                    {
                        List<Model.Guest> selectedGuests = (List<Model.Guest>)dgGuests.ItemsSource;
                        bool roomAvailable = CheckRoomAvailability(roomId, startDateTime);

                        if (!roomAvailable)
                        {
                            MessageBox.Show("Room is already booked for this period.");
                            return;
                        }

                        if (contextReservation != null) 
                        {

                            contextReservation.RoomId = roomId;
                            contextReservation.ReservationType = reservationType;
                            contextReservation.Guests = selectedGuests;
                            contextReservation.StartDateTime = startDateTime;
                            contextReservation.EndDateTime = null;
                            contextReservation.TotalPrice = null;

                            reservationService.SaveReservation(contextReservation);
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Reservation Type");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid date");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Reservation Type");
                }
            }
            else
            {
                MessageBox.Show("The field(s) can't be empty.");
            }
        }

        public bool CheckRoomAvailability(int roomId, DateTime startDateTime, DateTime? endDateTime = null)
        {
            List<Reservation> reservationsForRoom = reservationService.GetReservationsForRoom(roomId);

            bool roomAvailable;

            if (endDateTime.HasValue)
            {
                roomAvailable = reservationsForRoom.All(reservation =>
                {
                    bool overlaps = (startDateTime < reservation.EndDateTime) && (endDateTime.Value > reservation.StartDateTime);
                    return !overlaps;
                });
            }
            else
            {
                roomAvailable = reservationsForRoom.All(reservation =>
                {
                    bool overlaps = startDateTime < reservation.EndDateTime;
                    return !overlaps;
                });
            }

            return roomAvailable;
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

