using System;

namespace HotelReservations.Model
{
    public class Price
    {
        private double price = 0;
        public int Id { get; set; }
        public RoomType RoomType { get; set; }
        public ReservationType ReservationType { get; set; }
        public double PriceValue { get; set; }
        
        public bool IsActive { get; set; } = true;


        public override string ToString()
        {
            return $"({Id + ", " +RoomType + ", " + ReservationType + ", " + PriceValue})";
        }
    }
}
