using HotelReservations.Model;
using HotelReservations.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Service
{
    public class RoomService
    {
        RoomRepository roomRepository;
        private ReservationService reservationService = new ReservationService();
        private RoomTypeRepository roomTypeRepository = new RoomTypeRepository();
        public RoomService() 
        { 
            roomRepository = new RoomRepository();
        }

        public List<Room> GetAllRooms()
        {
            var instanceOfRooms = Hotel.GetInstance().Rooms;
            List<Room> roomList = new List<Room>();
            foreach (var rooms in instanceOfRooms)
            {
                if (rooms.IsActive == true)
                {
                    roomList.Add(rooms);
                }
            }

            return roomList;
        }

        public List<Room> GetSortedRooms()
        {
            var rooms = GetAllRooms();
            rooms.Sort((r1, r2) => r1.RoomNumber.CompareTo(r2.RoomNumber));
            return rooms;
        }

        public List<Room> GetAllRoomsByRoomNumber(string startingWith)
        {
            var rooms = GetAllRooms();
            var filteredRooms = rooms.FindAll((r) => r.RoomNumber.StartsWith(startingWith));
            return filteredRooms;
        }

        public void SaveRoom(Room room)
        {
            if (room.Id == 0)
            {
                room.Id = GetNextIdValue();
                Hotel.GetInstance().Rooms.Add(room);
            }
            else
            {
                var index = Hotel.GetInstance().Rooms.FindIndex(r => r.Id == room.Id);
                Hotel.GetInstance().Rooms[index] = room;
            }
            roomRepository.Save(Hotel.GetInstance().Rooms);

        }

        public int GetNextIdValue()
        {
            if (Hotel.GetInstance().Rooms.Any())
            {
                return Hotel.GetInstance().Rooms.Max(r => r.Id) + 1;
            }
            else
            {

                return 1;
            }
        }

        public List<RoomType> GetAllRoomTypes()
        {
            List<RoomType> roomTypes = new List<RoomType>();
            var hotel = Hotel.GetInstance().RoomTypes;
            foreach (var type in hotel)
            {
                roomTypes.Add(type);
            }

            return roomTypes;

        }
        public List<RoomType> GetAllActiveRoomTypes()
        {
            List<RoomType> roomTypes = new List<RoomType>();
            var hotel = Hotel.GetInstance().RoomTypes;
            foreach (var type in hotel)
            {
                if (type.IsActive) { 
                    roomTypes.Add(type);
                }
            }

            return roomTypes;

        }

        public bool ReservationExistsForRoom(Room room)
        {
            var reservations = reservationService.GetReservationsForRoom(room.Id);
            if (reservations.Count > 0)
            {
                return true;
            }
            return false;
        }
       


        public RoomType GetRoomTypeById(int id)
        {
           var rooms = GetAllRooms();
           foreach (var room in rooms)
            {
                if (room.Id == id)
                {
                    return room.RoomType;
                }
            }
            return null;
        }

       



        public void SaveRoomType(RoomType roomType)
        {
            if (roomType.Id == 0)
            {
                roomType.Id = GetNextIdValueForRT();
                Hotel.GetInstance().RoomTypes.Add(roomType);
            }
            else
            {
                var index = Hotel.GetInstance().RoomTypes.FindIndex(r => r.Id == roomType.Id);
                Hotel.GetInstance().RoomTypes[index] = roomType;
            }

            roomTypeRepository.Save(Hotel.GetInstance().RoomTypes);
        }

        public int GetNextIdValueForRT()
        {
            if (Hotel.GetInstance().RoomTypes.Any())
            {
                return Hotel.GetInstance().RoomTypes.Max(r => r.Id) + 1;
            }
            else
            {

                return 1;
            }
        }

        public List<Room> GetRoomsByType(RoomType roomType)
        {
            List<Room> list = GetAllRooms();
            var roomsByType = list.Where(rt=> rt.RoomType == roomType).ToList();
            return roomsByType;
        }
    }
}
