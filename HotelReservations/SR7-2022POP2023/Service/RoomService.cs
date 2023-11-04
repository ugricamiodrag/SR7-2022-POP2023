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
        IRoomRepository roomRepository;
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

        public void SaveRoom(Room newRoom)
        {
            Hotel.GetInstance().Rooms.Add(newRoom);
        }

        public int GetNextIdValue()
        {
            return Hotel.GetInstance().Rooms.Max(r => r.Id) + 1;
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

        public RoomType GetRoomTypeByName(string roomTypeName)
        {
            return Hotel.GetInstance().RoomTypes.FirstOrDefault(rt => rt.Name == roomTypeName);
        }

        public void UpdateRoom(Room room)
        {
            var rooms = roomRepository.Load();
            if (rooms != null)
            {
                int index = rooms.FindIndex(r => r.Id == room.Id);
                if (index >= 0)
                {
                    rooms[index] = room;
                    roomRepository.Save(rooms);
                }
            }
        }
    }
}
