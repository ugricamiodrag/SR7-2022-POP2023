
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Exceptions
{
    public class CouldntPersistDataException : Exception
    {
        public CouldntPersistDataException() { }
        public CouldntPersistDataException(string message) : base(message) { }
    }
}
