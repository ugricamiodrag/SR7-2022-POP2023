using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    public interface IRoomTypeRepository
    {
        List<RoomType> Load();
        void Save(List<RoomType> roomTypes);
        int Insert(RoomType roomType);

        void Update(RoomType roomType);
    }
}
