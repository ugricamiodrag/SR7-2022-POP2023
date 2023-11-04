using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    [Serializable]
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public bool HasTV { get; set; } 
        public bool HasMiniBar { get; set; }
        public RoomType RoomType { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            //return "Room number: " + RoomNumber; // ...
            return $"Room number: {RoomNumber}";
        }
    }
}
