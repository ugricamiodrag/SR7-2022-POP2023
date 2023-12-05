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
    public class PriceListRepository : IPriceListRepository
    {
        public int Insert(Price price)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [dbo].[price_list] (price_list_room_type_id, price_list_reservation_type, price_list_value, price_list_is_active)
                    OUTPUT inserted.price_list_id
                    VALUES (@price_list_room_type_id, @price_list_reservation_type, @price_list_value, @price_list_is_active)
                "
                ;

                command.Parameters.Add(new SqlParameter("price_list_room_type_id", price.RoomType.Id));
                command.Parameters.Add(new SqlParameter("price_list_reservation_type", price.ReservationType.ToString()));
                command.Parameters.Add(new SqlParameter("price_list_value", price.PriceValue));
                command.Parameters.Add(new SqlParameter("price_list_is_active", price.IsActive));
                


                return (int)command.ExecuteScalar();
            }
        }

        public List<Price> Load()
        {
            var priceList = new List<Price>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT r.*, rt.* FROM [dbo].[price_list] r\r\nINNER JOIN [dbo].[room_type] rt ON r.price_list_room_type_id = rt.room_type_id";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "price");


                foreach (DataRow row in dataSet.Tables["price"]!.Rows)
                {
                    var price = new Price()
                    {
                        Id = (int)row["price_list_id"],
                        RoomType = new RoomType()
                        {
                            Id = (int)row["room_type_id"],
                            Name = (string)row["room_type_name"],
                            IsActive = (bool)row["room_type_is_active"]
                        },
                        ReservationType = (ReservationType)row["price_list_reservation_type"],
                        PriceValue = (double)row["price_list_value"],
                        IsActive = (bool)row["price_list_is_active"]

                    };

                    priceList.Add(price);
                }
            }

            return priceList;
        }

        public void Save(List<Price> priceList)
        {
           foreach(Price price in priceList)
            {
                if(price.Id == 0)
                {
                    Insert(price);
                }
                else
                {
                    Update(price);
                }
            }
        }

        public void Update(Price price)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE [dbo].[reservation]
                    SET price_list_room_type_id=@price_list_room_type_id, price_list_reservation_type=@price_list_reservation_type, price_list_value=@price_list_value, price_list_is_active=@price_list_is_active
                    WHERE price_list_id=@price_list_id
                "
                ;
                command.Parameters.Add(new SqlParameter("price_list_id", price.Id));
                command.Parameters.Add(new SqlParameter("price_list_room_type_id", price.RoomType.Id));
                command.Parameters.Add(new SqlParameter("price_list_reservation_type", price.ReservationType.ToString()));
                command.Parameters.Add(new SqlParameter("price_list_value", price.PriceValue));
                command.Parameters.Add(new SqlParameter("price_list_is_active", price.IsActive));

                command.ExecuteNonQuery();
            }
        }
    }
}
