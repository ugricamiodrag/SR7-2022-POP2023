using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    [Serializable]
    public class RoomType
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name { get

            { return name;  }
                 set
            { if  (name != value) { name = value;}
            else
                {
                    throw new Exception("It's required. ");
                }
            }
                }

        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return Name;
        }

        public RoomType Clone()
        {
            RoomType r = new RoomType();
            r.Id = Id;
            r.Name = Name;
            return r;
        }
    }
}
