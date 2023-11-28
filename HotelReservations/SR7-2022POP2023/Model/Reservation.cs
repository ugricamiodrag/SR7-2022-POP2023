using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public ReservationType ReservationType { get; set; }
        public List<Guest> Guests { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double TotalPrice { get; set; }
        public bool IsActive { get; set; } = true;
        
        public Reservation Clone()
        {
            Reservation reservation = new Reservation();
            reservation.Id = Id;
            reservation.RoomId = RoomId;
            reservation.ReservationType = ReservationType;
            reservation.Guests = Guests;
            reservation.StartDateTime = StartDateTime;
            reservation.EndDateTime = EndDateTime;
            reservation.TotalPrice = TotalPrice;
            reservation.IsActive = IsActive;
            return reservation;
        }
    }
}
