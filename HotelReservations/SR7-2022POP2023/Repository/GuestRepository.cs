using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using System.Diagnostics;

namespace HotelReservations.Repository
{
    public class GuestRepository : IGuestRepository
    {
        public int Insert(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [dbo].[guest] (guest_name, guest_surname, guest_id_number, guest_is_active)
                    OUTPUT inserted.guest_id
                    VALUES (@guest_name, @guest_surname, @guest_id_number, @guest_is_active)
                "
                ;

                command.Parameters.Add(new SqlParameter("guest_name", guest.Name));
                command.Parameters.Add(new SqlParameter("guest_surname", guest.Surname));
                command.Parameters.Add(new SqlParameter("guest_id_number", guest.IDNumber));
                command.Parameters.Add(new SqlParameter("guest_is_active", guest.IsActive));



                return (int)command.ExecuteScalar();
            }
        }

        public List<Guest> Load()
        {
            var guestList = new List<Guest>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[guest]";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "guest");


                foreach (DataRow row in dataSet.Tables["guest"]!.Rows)
                {
                    var guest = new Guest()
                    {
                        Id = (int)row["guest_id"],
                        Name = row["guest_name"] as string,
                        Surname = row["guest_surname"] as string,
                        IDNumber = row["guest_id_number"] as string,
                        IsActive = (bool)row["guest_is_active"]

                    };

                    guestList.Add(guest);
                }
            }

            return guestList;
        }

        public void Save(List<Guest> guests)
        {
            foreach (Guest guest in guests)
            {
                if (Exists(guest.Id))
                {
                    Update(guest);
                    
                }
                else
                {
                    Insert(guest);
                }
            }
        }

        public bool Exists(int guestId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [dbo].[guest] WHERE guest_id = @guest_id";
                command.Parameters.AddWithValue("@guest_id", guestId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }


        public void Update(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE [dbo].[guest]
                    SET guest_name=@guest_name, guest_surname=@guest_surname, guest_id_number=@guest_id_number, guest_is_active=@guest_is_active
                    WHERE guest_id=@guest_id
                "
                ;
                command.Parameters.Add(new SqlParameter("guest_id", guest.Id));
                command.Parameters.Add(new SqlParameter("guest_name", guest.Name));
                command.Parameters.Add(new SqlParameter("guest_surname", guest.Surname));
                command.Parameters.Add(new SqlParameter("guest_id_number", guest.IDNumber));
                command.Parameters.Add(new SqlParameter("guest_is_active", guest.IsActive));

                command.ExecuteNonQuery();
            }
        }
    }
}
