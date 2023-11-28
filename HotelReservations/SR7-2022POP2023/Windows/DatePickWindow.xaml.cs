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
    /// <summary>
    /// Interaction logic for DatePickWindow.xaml
    /// </summary>
    public partial class DatePickWindow : Window
    {

        public event EventHandler<DateTime> DateSelected;

        public DatePickWindow()
        {
            InitializeComponent();
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = datePicker1.SelectedDate ?? DateTime.Now;

            DateSelected?.Invoke(this, selectedDate);

            Close();
        }
    }
}
