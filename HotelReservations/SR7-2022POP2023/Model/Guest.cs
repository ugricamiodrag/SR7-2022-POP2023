using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    public class Guest
    {
        public int Id { get; set; }

        private string name = String.Empty;
        private string surname = string.Empty;

        public string Name { get
            { return name;  }
                
                set
            { if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required.");
                } 
            name = value;}
                }
        public string Surname {
            get
            { return surname; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required.");
                }
                surname = value;
            }
        }
        public string IDNumber { get; set; }
        public bool IsActive { get; set; } = true;

        public Guest Clone()
        {
            var clone = new Guest();
            clone.Id = Id;
            clone.Name = Name;
            clone.Surname = Surname;
            clone.IDNumber = IDNumber;
            clone.IsActive = IsActive;
            return clone;


        }

        public override string ToString()
        {
            return $"Name: {Name} \nSurname: {Surname} \nID Number: {IDNumber}\n";
        }

    }

    
}
