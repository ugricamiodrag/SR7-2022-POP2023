using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Repository;

namespace HotelReservations.Service
{
    public class GuestService
    {
        GuestRepository guestRepository;
        public GuestService() { 
            
            guestRepository = new GuestRepository();
        
        }

        public List<Guest> GetAllActiveGuests()
        {
            var allGuests = Hotel.GetInstance().Guests;
            List<Guest> result = new List<Guest>();
            foreach (var guest in allGuests)
            {
                if (guest.IsActive == true)
                {
                    result.Add(guest);
                }
            }

            return result;

        }

        public void SaveGuest(Guest guest)
        {
            if (guest.Id == 0)
            {
                guest.Id = GetNextIdValue();
                guest.IDNumber = GenerateIDNumber(guest.Name, guest.Surname, guest.Id);
                Hotel.GetInstance().Guests.Add(guest);
            }
            else
            {
                var index = Hotel.GetInstance().Guests.FindIndex(r => r.Id == guest.Id);
                Hotel.GetInstance().Guests[index] = guest;
            }
        }

        private int GetNextIdValue()
        {
            var maxId = Hotel.GetInstance().Guests.Any() ? Hotel.GetInstance().Guests.Max(r => r.Id) : 0;
            return maxId + 1;
        }


        private string GenerateIDNumber(string name, string surname, int id)
        {
            return $"{name.Substring(0, 1)}{surname.Substring(0, 1)}-{id:D5}";
        }
    }
}
