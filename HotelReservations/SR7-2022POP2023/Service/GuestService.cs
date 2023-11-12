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
            var allGuests = guestRepository.Load();
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
                Hotel.GetInstance().Guests.Add(guest);
            }
            else
            {
                var index = Hotel.GetInstance().Guests.FindIndex(r => r.Id == guest.Id);
                Hotel.GetInstance().Guests[index] = guest;
            }
        }

        public int GetNextIdValue()
        {
            return Hotel.GetInstance().Guests.Max(r => r.Id) + 1;
        }
    }
}
