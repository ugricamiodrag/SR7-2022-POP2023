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

        private string roomNumber = string.Empty;
        public string RoomNumber
        {
            get { return roomNumber; }
            set { 

                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("It's required");
                }

                roomNumber = value;
            }
        }
        public bool HasTV { get; set; } 
        public bool HasMiniBar { get; set; }
        private RoomType roomType;
        public RoomType RoomType
        {
            get { return roomType; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Room type cannot be null");
                }

                roomType = value;
            }
        }
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            //return "Room number: " + RoomNumber; // ...
            return $"Room number: {RoomNumber}";
        }


        public Room Clone()
        {
            var clone = new Room();
            clone.Id = Id;
            clone.RoomNumber = RoomNumber;
            clone.HasTV = HasTV;
            clone.HasMiniBar = HasMiniBar;
            clone.RoomType = RoomType;
            clone.IsActive = IsActive;

            return clone;
        }
    }
}
