using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    public interface IReservationRepository
    {

        List<Reservation> Load();
        void Save(List<Reservation> reservationList);
    }
}
