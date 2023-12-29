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
            ReservationDG.ItemsSource = null;
            ReservationDG.ItemsSource = view;
            ReservationDG.IsSynchronizedWithCurrentItem = true;

            ReservationDG.SelectedIndex = -1;
            view.Refresh();
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

            if (selectedRes != null)
            {
                var editResWindow = new AddEditReservation(selectedRes);

                Hide();

                if (editResWindow.ShowDialog() == true)
                {
                    FillData();
                }

                Show();
            }
            else
            {
                MessageBox.Show("You didn't pick a reservation.");
            }
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


            if (e.PropertyName == "EndDateTime")
            {
                e.Column.Visibility = Visibility.Collapsed;

                DataGridTemplateColumn leaveRoomColumn = new DataGridTemplateColumn();
                leaveRoomColumn.Header = "Leave Room";

                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonFactory.SetValue(Button.ContentProperty, "Leave Room");
                buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(btnLeaveRoom_Click));

                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = buttonFactory;

                leaveRoomColumn.CellTemplate = dataTemplate;
                ReservationDG.Columns.Add(leaveRoomColumn);
            }
        }

        private void btnLeaveRoom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var reservation = button.DataContext as Reservation; 
                if (reservation != null)
                {

                    reservation.EndDateTime = DateTime.Now;


                    CalculateTotalPrice(reservation);
                }
            }
        }

        private void CalculateTotalPrice(Reservation reservation)
        {
            if (reservation != null)
            {
                DateTime startDate = reservation.StartDateTime;
                DateTime endDate = DateTime.Now; 
                var sk = endDate - startDate;
                int difference = (int)sk.TotalDays;
                int roomId = reservation.RoomId;

                if (difference > 0)
                {
                    var roomType = roomService.GetRoomTypeById(roomId);
                    ReservationType resType = difference > 1 ? ReservationType.Night : ReservationType.Day;
                    List<Price> price = GetPricesForRoomType(roomType, resType);

                    double priceValue = price.FirstOrDefault()?.PriceValue ?? 0; 

                    var totalPrice = priceValue * difference;
                    reservation.TotalPrice = totalPrice;
                }
                else
                {
                    MessageBox.Show("Pick the right dates.");
                }
            }
            else
            {
                MessageBox.Show("Reservation data is empty.");
            }
        }

        private List<Price> GetPricesForRoomType(RoomType roomType, ReservationType reservationType)
        {

            List<Price> pricesList = priceListService.GetAllPrices();
            var pricesForRoom = pricesList.Where(price =>
                price.RoomType.Name == roomType.Name && price.ReservationType == reservationType && price.IsActive == true).ToList();

            return pricesForRoom;
        }



    }
}



