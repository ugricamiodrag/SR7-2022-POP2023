using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    interface IGuestRepository
    {
        List<Guest> Load();

        void Save(List<Guest> guests);
    }
    
}
