using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Repository;

namespace HotelReservations.Service
{
    public class ReservationService
    {
        private ReservationRepository reservationRepository = new ReservationRepository();
        public List<Reservation> getAllReservations()
        {
            var res = Hotel.GetInstance().Reservations;
            List<Reservation> all = new List<Reservation>();
            foreach (Reservation reservation in res)
            {
                if (reservation.IsActive == true)
                {
                    all.Add(reservation);
                }
            }
            return all;
        }

        public void SaveReservation(Reservation reservation)
        {
            if (reservation.Id == 0)
            {
                reservation.Id = GetNextIdValue();
                Hotel.GetInstance().Reservations.Add(reservation);
            }
            else
            {
                var index = Hotel.GetInstance().Reservations.FindIndex(r => r.Id == reservation.Id);
                Hotel.GetInstance().Reservations[index] = reservation;
            }
            reservationRepository.Save(Hotel.GetInstance().Reservations);

        }

        public int GetNextIdValue()
        {
            if (Hotel.GetInstance().Reservations.Any())
            {
                return Hotel.GetInstance().Reservations.Max(r => r.Id) + 1;
            }
            else
            {

                return 1;
            }
        }

        public List<Reservation> GetReservationsForRoom(int roomId)
        {
            List<Reservation> reservations = new List<Reservation>();
            List<Reservation> all = getAllReservations();
            foreach (Reservation reservation in all)
            {
                if (reservation.RoomId == roomId)
                {
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }

        public List<Reservation> GetReservationsByRoomType(RoomType roomType)
        {
            var roomService = new RoomService(); 
            var roomsOfType = roomService.GetRoomsByType(roomType); 

            var allReservations = getAllReservations(); 
            var reservationsForRoomType = new List<Reservation>();

            foreach (var room in roomsOfType)
            {
                var reservationsForRoom = allReservations.Where(reservation => reservation.RoomId == room.Id);
                reservationsForRoomType.AddRange(reservationsForRoom);
            }

            return reservationsForRoomType;
        }

       
    }
}
