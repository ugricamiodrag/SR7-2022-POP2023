using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Exceptions;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    class GuestRepository : IGuestRepository
    {
        private string ToCSV(Guest guest)
        {
            return $"{guest.Id},{guest.Name},{guest.Surname},{guest.IDNumber},{guest.IsActive}";
        }

        private Guest FromCSV(string csv)
        {
            string[] parts = csv.Split(',');
            Guest guest = new Guest();
            guest.Id = int.Parse(parts[0]);
            guest.Name = parts[1];
            guest.Surname = parts[2];
            guest.IDNumber = parts[3];
            guest.IsActive = bool.Parse(parts[4]);

            return guest;

        }

        public List<Guest> Load()
        {
            if (!File.Exists("guests.txt"))
            {
                return null;
            }

            try
            {
                using (var streamReader = new StreamReader("guests.txt"))
                {
                    List<Guest> guests = new List<Guest>();
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var room = FromCSV(line);
                        guests.Add(room);
                    }

                    return guests;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new CouldntLoadResourceException(ex.Message);
            }
        }

        public void Save(List<Guest> guests)
        {
            try
            {
                using (var streamWriter = new StreamWriter("guests.txt"))
                {
                    foreach (var guest in guests)
                    {
                        streamWriter.WriteLine(ToCSV(guest));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CouldntPersistDataException(ex.Message);
            }
        }
    }
}
