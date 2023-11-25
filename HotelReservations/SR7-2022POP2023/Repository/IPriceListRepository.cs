using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;

namespace HotelReservations.Repository
{
    public interface IPriceListRepository
    {
        List<Price> Load();

        void Save(List<Price> priceList);
    }
}
