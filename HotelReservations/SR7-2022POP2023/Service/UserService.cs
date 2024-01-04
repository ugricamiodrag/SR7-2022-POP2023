using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Repository;

namespace HotelReservations.Service
{
    public class UserService
    {
        private UserRepository userRepository = new UserRepository();
        public List<User> GetAllUsers()
        {
            return Hotel.GetInstance().Users;
        }

        public List<User> GetAllActiveUsers()
        {
            var users = Hotel.GetInstance().Users;
            List<User> result = new List<User>();
            foreach (var user in users)
            {
                if (user.IsActive == true)
                {
                    result.Add(user);
                }
            }
            return result;
        }

        public void SaveUser(User user, string type)
        {
            if (user.Id == 0)
            {
                user.Id = GetNextIdValue();
                user.UserType = type; 
                Hotel.GetInstance().Users.Add(user);
            }
            else
            {
                var index = Hotel.GetInstance().Users.FindIndex(u => u.Id == user.Id);
                if (index != -1)
                {
                    user.UserType = type; 
                    Hotel.GetInstance().Users[index] = user;
                }
            }

          
        }

        public User ReturnUser(string username,  string password)
        {
            var users = GetAllActiveUsers();
            foreach (var user in users)
            {
                if (user.Username.Equals(username) && user.Password.Equals(password))
                {
                    return user;

                }
            }
            return null!;
        }

        public int GetNextIdValue()
        {
            if (Hotel.GetInstance().Users.Any())
            {
                return Hotel.GetInstance().Users.Max(r => r.Id) + 1;
            }
            else
            {
        
                return 1;
            }
        }

        public bool isUsernameTaken(String username)
        {
            List<User> allUsers = GetAllActiveUsers();
            foreach (var user in allUsers)
            {
                if (user.Username.Equals(username))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
