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
    /// Interaction logic for PickGuestWindow.xaml
    /// </summary>
    public partial class PickGuestWindow : Window
    {
        private GuestService guestService = new GuestService();
        private List<Model.Guest> selectedGuests = new List<Model.Guest>(); 

        public PickGuestWindow()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            List<Model.Guest> availableGuests = guestService.GetAllActiveGuests();
            GuestListBox.ItemsSource = availableGuests;
        }

        private void GuestListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Model.Guest selectedGuest in e.RemovedItems)
            {
                selectedGuests.RemoveAll(guest => guest.Id == selectedGuest.Id);
            }

            foreach (Model.Guest selectedGuest in e.AddedItems)
            {
                if (!selectedGuests.Any(guest => guest.Id == selectedGuest.Id))
                {
                    selectedGuests.Add(selectedGuest);
                }
            }

           
            GuestDetailsTextBox.Text = string.Join(Environment.NewLine, selectedGuests.Select(guest => guest.ToString()));
        }


        public Action<string> GuestIdSetter { get; set; }

        private void btnPickAGuest_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGuests.Any() && GuestIdSetter != null)
            {
                string guestIds = string.Join("|", selectedGuests.Select(guest => guest.Id));
                GuestIdSetter.Invoke(guestIds);
                this.Close();
            }
        }
    }

}
