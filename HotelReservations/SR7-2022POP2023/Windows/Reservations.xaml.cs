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
using HotelReservations.Windows;

namespace HotelReservations.Windows
{
    /// <summary>
    /// Interaction logic for Reservations.xaml
    /// </summary>
    public partial class Reservations : Window
    {
        private ICollectionView view;
        private RoomService roomService = new RoomService();
        private PriceListService priceListService = new PriceListService();
        private bool isLeaveRoomColumnAdded = false;
        public Reservations()
        {
            InitializeComponent();
            FillData();
        }
        private void FillData()
        {
            var resService = new ReservationService();
            var allRes = resService.getAllReservations();

            view = CollectionViewSource.GetDefaultView(allRes);
            view.Filter = DoFilter;


            ReservationDG.ItemsSource = null;
            ReservationDG.ItemsSource = view;
            ReservationDG.IsSynchronizedWithCurrentItem = true;

            ReservationDG.SelectedIndex = -1;
            view.Refresh();
        }

        private bool DoFilter(object reservationObject)
        {
            var reservation = reservationObject as Reservation;

            var roomNumberSearchParam = RoomNumberSearchTB.Text.ToLower();
            var arrivalDateSearchParam = StartDatePicker.SelectedDate;
            var departureDateSearchParam = EndDatePicker.SelectedDate;
            var reservationRoomNumber = roomService.getRoomById(reservation.RoomId);

            bool matchesRoomNumber = string.IsNullOrEmpty(roomNumberSearchParam) ||
                                     reservationRoomNumber.RoomNumber.ToString().Contains(roomNumberSearchParam);

            bool matchesArrivalDate = arrivalDateSearchParam == null ||
                                      reservation.StartDateTime.Date == arrivalDateSearchParam.Value.Date;

            bool matchesDepartureDate = departureDateSearchParam == null ||
                                        reservation.EndDateTime?.Date == departureDateSearchParam.Value.Date;

            return matchesRoomNumber && matchesArrivalDate && matchesDepartureDate;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var addReservation = new AddEditReservation();

            Hide();
            if (addReservation.ShowDialog() == true)
            {
                FillData();
            }
            Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRes = (Reservation)view.CurrentItem;

            if (selectedRes == null)
            {
                MessageBox.Show("You didn't pick a reservation.");
                return;
            }

            if (selectedRes.EndDateTime.HasValue)
            {
                MessageBox.Show("You can't edit this reservation.");
                return;
            }

            var editResWindow = new AddEditReservation(selectedRes);

            Hide();

            if (editResWindow.ShowDialog() == true)
            {
                FillData();
            }

            Show();
        }


        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var resToDelete = (Reservation)ReservationDG.SelectedItem;
            if (resToDelete != null)
            {
                var decision = MessageBox.Show($"Do you want to delete the reservation {resToDelete.Id}", "Deleting a reservation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (decision == MessageBoxResult.Yes)
                {
                    resToDelete.IsActive = false;
                    FillData();
                }

            }
            else
            {
                MessageBox.Show("You didn't pick a reservation.");
            }
        }

        private void ReservationDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.ToLower() == "IsActive".ToLower())
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            if (e.PropertyName == "Guests")
            {
                e.Column.Visibility = Visibility.Collapsed;

                var existingGuestsColumns = ReservationDG.Columns.Where(c => c.Header.ToString() == "All guests").ToList();
                foreach (var column in existingGuestsColumns)
                {
                    ReservationDG.Columns.Remove(column);
                }

                DataGridTemplateColumn guestsColumn = new DataGridTemplateColumn();
                guestsColumn.Header = "All guests";

                FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Guests") { Converter = new GuestListConverter() });

                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = textBlockFactory;

                guestsColumn.CellTemplate = dataTemplate;
                ReservationDG.Columns.Add(guestsColumn);
            }


   

        }

     

       

        private List<Price> GetPricesForRoomType(RoomType roomType, ReservationType reservationType)
        {

            List<Price> pricesList = priceListService.GetAllPrices();
            var pricesForRoom = pricesList.Where(price =>
                price.RoomType.Name == roomType.Name && price.ReservationType == reservationType && price.IsActive == true).ToList();

            return pricesForRoom;
        }

        private void CheckOutGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            var dateNow = DateTime.Now;
            var selectedReservation = (Reservation)ReservationDG.SelectedItem;

            if (selectedReservation != null)
            {
                if (!selectedReservation.IsActive)
                {
                    MessageBox.Show("This reservation is already checked out.");
                    return;
                }

                if(dateNow < selectedReservation.StartDateTime)
                {
                    MessageBox.Show("You can't perform this action at the time.");
                    return;
                }


                var checkOutConfirmation = MessageBox.Show("Are you sure you want to check out this reservation?", "Checkout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (checkOutConfirmation == MessageBoxResult.Yes)
                {
                    var checkOutWindow = new CheckOutGuestsHandler(selectedReservation);
                    FillData();
                }
            }
            else
            {
                MessageBox.Show("Please select a reservation to check out.");
            }
        }

        private void RoomNumberSearchTB_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            view.Refresh();
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            view.Refresh();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            view.Refresh();
        }
    }
}



