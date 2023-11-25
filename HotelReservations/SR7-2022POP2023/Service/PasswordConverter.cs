using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HotelReservations.Service
{
    public class PasswordConverter : IValueConverter
    {
        private int maxLength;

        public PasswordConverter(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string password = value as string;
            if (!string.IsNullOrEmpty(password))
            {
                return new string('*', Math.Min(password.Length, maxLength));
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
