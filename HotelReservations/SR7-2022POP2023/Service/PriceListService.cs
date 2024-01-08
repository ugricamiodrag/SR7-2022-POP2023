using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Repository;

namespace HotelReservations.Service
{
    public class PriceListService
    {
        private PriceListRepository listRepository = new PriceListRepository();
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

        public bool DoesPriceExistForRoomAndReservationType(Price contextPrice)
        {
            var allPrices = GetAllPrices();
            return allPrices.Any(price =>
                price.RoomType.Id == contextPrice.RoomType.Id &&
                price.ReservationType == contextPrice.ReservationType &&
                price.IsActive);
        }


        public List<Price> GetPricesByRoomType(RoomType roomType)
        {
            var allPrices = GetAllPrices();
            return allPrices.Where(price => price.RoomType == roomType).ToList();
        }
    }
}
