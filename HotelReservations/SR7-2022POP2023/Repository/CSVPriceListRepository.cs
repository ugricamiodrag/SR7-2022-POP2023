using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HotelReservations.Exceptions;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    public class CSVPriceListRepository : IPriceListRepository
    {

        private string ToCSV(Price price)
        {
            return $"{price.Id},{price.RoomType.Id},{price.ReservationType},{price.PriceValue},{price.IsActive}";

        }

        private Price FromCSV(string csv)
        {
            string[] tokens = csv.Split(',');
            var price = new Price();
            price.Id = int.Parse(tokens[0]);
            var roomTypeId = int.Parse(tokens[1]);
            var roomTypes = Hotel.GetInstance().RoomTypes;
            if (roomTypes != null)
            {
                price.RoomType = roomTypes.Find(rt => rt.Id == roomTypeId)!;
            }
            else
            {
                MessageBox.Show("There has been an error. Contact an administrator.");
            }
            if (Enum.TryParse(typeof(ReservationType), tokens[2], out object reservationType))
            {
                price.ReservationType = (ReservationType)reservationType;
            }
            else
            {
                throw new Exception("There is an error, try again later.");
            }
            price.PriceValue = double.Parse(tokens[3]);
            price.IsActive = bool.Parse(tokens[4]);

            return price;

        }

        //public List<Price> Load()
        //{
        //    if (!File.Exists("pricelist.txt"))
        //    {
        //        return null!;
        //    }

        //    try
        //    {
        //        using (var streamReader = new StreamReader("pricelist.txt"))
        //        {
        //            List<Price> priceList = new List<Price>();
        //            string line;

        //            while ((line = streamReader.ReadLine()!) != null)
        //            {
        //                var prices = FromCSV(line);
        //                priceList.Add(prices);
        //            }

        //            return priceList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw new CouldntLoadResourceException(ex.Message);
        //    }
        //}

        public void Save(List<Price> priceList)
        {
            try
            {
                using (var streamWriter = new StreamWriter("pricelist.txt"))
                {
                    foreach (var price in priceList)
                    {
                        streamWriter.WriteLine(ToCSV(price));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CouldntPersistDataException(ex.Message);
            }
        }

        public int Insert(Price price)
        {
            throw new NotImplementedException();
        }

        public void Update(Price price)
        {
            throw new NotImplementedException();
        }

        public List<Price> Load()
        {
            throw new NotImplementedException();
        }
    }
}
