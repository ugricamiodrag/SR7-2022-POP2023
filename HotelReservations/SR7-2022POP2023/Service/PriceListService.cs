using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Service
{
    public class PriceListService
    {

        public List<Price> GetAllPrices()
        {
            var prices = Hotel.GetInstance().PriceList;
            List<Price> priceList = new List<Price>();
            foreach (var price in prices)
            {
                if (price.IsActive == true)
                {
                    priceList.Add(price);
                }
            }
            return priceList;
        }

        public void SavePrice(Price price)
        {
            if (price.Id == 0)
            {
                price.Id = GetNextIdValue();
                Hotel.GetInstance().PriceList.Add(price);
            }
            else
            {
                var index = Hotel.GetInstance().PriceList.FindIndex(r => r.Id == price.Id);
                Hotel.GetInstance().PriceList[index] = price;
            }
        }

        public int GetNextIdValue()
        {
            if (Hotel.GetInstance().PriceList.Any())
            {
                return Hotel.GetInstance().PriceList.Max(r => r.Id) + 1;
            }
            else
            {

                return 1;
            }
        }

        public List<ReservationType> reservationTypes = new List<ReservationType>() { ReservationType.Day, ReservationType.Night};

        public List<RoomType> roomTypesFromPriceList()
        {
            var allPrices = GetAllPrices();
            List<RoomType> roomTypes = new List<RoomType>();
            foreach (var price in allPrices)
            {
                roomTypes.Add(price.RoomType);
            }
            return roomTypes;
        }

        public List<ReservationType> roomReservationTypeFromPriceList()
        {
            var allPrices = GetAllPrices();
            List<ReservationType> reservationTypes = new List<ReservationType>();
            foreach (var price in allPrices)
            {
                reservationTypes.Add(price.ReservationType);
            }
            return reservationTypes;
        }


    }
}
