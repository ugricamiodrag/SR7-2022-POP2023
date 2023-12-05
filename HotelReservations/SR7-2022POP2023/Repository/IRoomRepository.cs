using HotelReservations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Repository
{
    public interface IRoomRepository
    {
        List<Room> Load();
        int Insert(Room room);
        void Update(Room room);
        void Save(List<Room> roomList);
    }
}
