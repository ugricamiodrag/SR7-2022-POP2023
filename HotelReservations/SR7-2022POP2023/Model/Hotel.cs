using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    public class Hotel
    {
        public int Id { get; set; }
        private string name;
        public string Name { 
            get { return name; }
            set
            {
                if (value != null && value != "")
                { name = value; }
                else { throw new ArgumentException(); }
            }
        }

        public string Address { get; set; }

        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Price> PriceList { get; set; } = new List<Price>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        private Hotel() 
        { 
        }

        private static Hotel instance;
        public static Hotel GetInstance()
        {
            if(instance == null)
            {
                instance = new Hotel();
            }
            return instance;
        }

    }
}
