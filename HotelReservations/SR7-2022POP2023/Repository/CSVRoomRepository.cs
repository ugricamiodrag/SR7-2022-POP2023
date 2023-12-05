using HotelReservations.Exceptions;
using HotelReservations.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Repository
{
    // Koristimo repozitorijume da bismo izolovali operacije
    // vezane za eksternu memoriju. Sad koristimo csv, sutra bazu,
    // potrebno nam je lako uvođenje promena.
    // Na narednim terminima ćemo "apstrahovati" repozitorijume
    // interfejsima.
    public class CSVRoomRepository : IRoomRepository
    {
        private string ToCSV(Room room)
        {
            return $"{room.Id},{room.RoomNumber},{room.HasTV},{room.HasMiniBar},{room.RoomType.Id},{room.IsActive}";
        }

        private Room FromCSV(string csv)
        {
            string[] csvValues = csv.Split(',');

            var room = new Room();
            room.Id = int.Parse(csvValues[0]);
            room.RoomNumber = csvValues[1];
            room.HasTV = bool.Parse(csvValues[2]);
            room.HasMiniBar = bool.Parse(csvValues[3]);
            var roomTypeId = int.Parse(csvValues[4]);
            room.RoomType = Hotel.GetInstance().RoomTypes.Find(rt => rt.Id == roomTypeId)!;
            room.IsActive = bool.Parse(csvValues[5]);

            return room;
        }


        private string ToCSV(RoomType roomType)
        {
            return $"{roomType.Id},{roomType.Name},{roomType.IsActive}";
        }

        private RoomType RTFromCSV(string csv)
        {
            string[] csvValues = csv.Split(",");

            var roomType = new RoomType();
            roomType.Id = int.Parse(csvValues[0]);
            roomType.Name = csvValues[1];
            roomType.IsActive = bool.Parse(csvValues[2]);

            return roomType;
        } 


        public void SaveRT(List<RoomType> roomTypes)
        {
            try
            {
                using (var streamWriter = new StreamWriter("roomtypes.txt"))
                {
                    foreach (var roomType in roomTypes)
                    {
                        streamWriter.WriteLine(ToCSV(roomType));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CouldntPersistDataException(ex.Message);
            }
        }

        public List<RoomType> GetRoomTypes() {
            if (!File.Exists("roomtypes.txt"))
            {
                return null!;
            }

            try
            {
                using (var streamReader = new StreamReader("roomtypes.txt"))
                {
                    List<RoomType> roomTypes = new List<RoomType>();
                    string line;

                    while ((line = streamReader.ReadLine()!) != null)
                    {
                        var roomType = RTFromCSV(line);
                        roomTypes.Add(roomType);
                    }

                    return roomTypes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new CouldntLoadResourceException(ex.Message);
            }
        }


        public void Save(List<Room> roomList)
        {
            try
            {
                using (var streamWriter = new StreamWriter("rooms.txt"))
                {
                    foreach (var room in roomList)
                    {
                        streamWriter.WriteLine(ToCSV(room));
                    }
                }
            }
            catch (Exception ex) 
            {
                throw new CouldntPersistDataException(ex.Message);
            }
            
        }

        public List<Room> Load()
        {
            if (!File.Exists("rooms.txt"))
            {
                return null!;
            }

            try
            {
                using (var streamReader = new StreamReader("rooms.txt"))
                {
                    List<Room> rooms = new List<Room>();
                    string line;

                    while ((line = streamReader.ReadLine()!) != null)
                    {
                        var room = FromCSV(line);
                        rooms.Add(room);
                    }

                    return rooms;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new CouldntLoadResourceException(ex.Message);
            }
        }

        public List<Room> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Insert(Room room)
        {
            throw new NotImplementedException();
        }

        public void Update(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
