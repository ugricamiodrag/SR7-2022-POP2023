using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations.Model
{
    // Popričali smo o 4 različite varijante kako je moguće
    // implementirati korisnike. Odabrali smo ovu kako biste
    // probali nasleđivanje u C# i utvrdili polimorfizam. 
    public class User
    {
        private string name = string.Empty;
        private string surname = string.Empty;
        private string jmbg = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;

        public int Id { get; set; }

        public string Name
        {
            get { return name; }
            set { if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentNullException("It's required");
                } 
                
                name = value; }
        }

        public string Surname
        {
            get { return surname; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required");
                }
                surname = value; }
        }

        public string JMBG
        {
            get { return jmbg; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required");
                }
                jmbg = value; }
        }

        public string Username
        {
            get { return username; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required");
                }
                username = value; }
        }

        public string Password
        {
            get { return password; }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("It's required");
                }
                password = value; }
        }

        public bool IsActive { get; set; } = true;


        public string UserType { get; set; }



        public User Clone()
        {
            var clone = new User();
            clone.username = Username;
            clone.password = Password;
            clone.name = Name;
            clone.JMBG = JMBG;
            clone.Id = Id;
            clone.surname = Surname;
            clone.UserType = UserType;
            return clone;
        }
    }
}
