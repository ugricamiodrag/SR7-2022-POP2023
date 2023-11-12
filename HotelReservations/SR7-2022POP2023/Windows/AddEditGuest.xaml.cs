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
    /// Interaction logic for AddEditGuest.xaml
    /// </summary>
    public partial class AddEditGuest : Window
    {
        private GuestService guestService;

        private Model.Guest contextGuest;
        public AddEditGuest(Model.Guest? guest = null)
        {
            if (guest == null)
            {
                contextGuest = new Model.Guest();
            }
            else
            {
                contextGuest = guest.Clone();
            }

            InitializeComponent();
            guestService = new GuestService();

            AdjustWindow(guest);

            this.DataContext = contextGuest;
        }

        public void AdjustWindow(Model.Guest? guest = null)
        {
            if (guest != null)
            {
                Title = "Edit Guest";
            }
            else
            {
                Title = "Add Guest";
            }

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            guestService.SaveGuest(contextGuest);

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
