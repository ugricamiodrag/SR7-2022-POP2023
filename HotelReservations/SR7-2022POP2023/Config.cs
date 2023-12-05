using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations
{
    public static class Config
    {
        public static string CONNECTION_STRING { get; } = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Projekat;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
    }
}
