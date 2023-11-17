using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    public interface IUserRepository
    {
        List<User> Load();

        void Save(List<User> guests);
    }
}
