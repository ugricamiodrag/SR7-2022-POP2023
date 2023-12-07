using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using HotelReservations.Windows;

namespace HotelReservations.Repository
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        public int Insert(RoomType roomType)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [dbo].[room_type] (room_type_name, room_type_is_active)
                    OUTPUT inserted.room_type_id
                    VALUES (@room_type_name, @room_type_is_active)
                "
                ;

                command.Parameters.Add(new SqlParameter("room_type_name", roomType.Name));
                command.Parameters.Add(new SqlParameter("room_type_is_active", roomType.IsActive));
               
                



                return (int)command.ExecuteScalar();
            }
        }

        public List<RoomType> Load()
        {
            var roomTypeList = new List<RoomType>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[room_type]";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "roomType");


                foreach (DataRow row in dataSet.Tables["roomType"]!.Rows)
                {
                    var roomType = new RoomType()
                    {
                        Id = (int)row["room_type_id"],
                        Name = row["room_type_name"] as string,
                        IsActive = (bool)row["room_type_is_active"]

                    };

                    roomTypeList.Add(roomType);
                }
            }

            return roomTypeList;
        }

        public void Save(List<RoomType> roomTypes)
        {
            foreach (RoomType roomType in roomTypes)
            {
                if (Exists(roomType.Id))
                {
                    Update(roomType);
                }
                else
                {
                    Insert(roomType);
                }
            }
        }

        public void Update(RoomType roomType)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE [dbo].[room_type]
                    SET room_type_name=@room_type_name, room_type_is_active=@room_type_is_active
                    WHERE room_type_id=@room_type_id
                "
                ;
                command.Parameters.Add(new SqlParameter("room_type_id", roomType.Id));
                command.Parameters.Add(new SqlParameter("room_type_name", roomType.Name));
                command.Parameters.Add(new SqlParameter("room_type_is_active", roomType.IsActive));

                command.ExecuteNonQuery();
            }
        }

        public bool Exists(int roomTypeId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [dbo].[room_type] WHERE room_type_id = @room_type_id";
                command.Parameters.AddWithValue("@room_type_id", roomTypeId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
