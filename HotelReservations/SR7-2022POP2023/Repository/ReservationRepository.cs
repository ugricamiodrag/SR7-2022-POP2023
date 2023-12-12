using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelReservations.Model;
using System.Globalization;

namespace HotelReservations.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        public int Insert(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [dbo].[reservation] (reservation_room_id, reservation_type, reservation_guests, reservation_start_date, reservation_end_date, reservation_total_price, reservation_is_active)
                    OUTPUT inserted.reservation_id
                    VALUES (@reservation_room_id, @reservation_type, @reservation_guests, @reservation_start_date, @reservation_end_date, @reservation_total_price, @reservation_is_active)
                "
                ;

                command.Parameters.Add(new SqlParameter("reservation_room_id", reservation.RoomId));
                command.Parameters.Add(new SqlParameter("reservation_type", reservation.ReservationType.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_guests", string.Join("|", reservation.Guests.Select(guest => guest.Id))));
                command.Parameters.Add(new SqlParameter("reservation_start_date", reservation.StartDateTime.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_end_date", reservation.EndDateTime.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_total_price", reservation.TotalPrice));
                command.Parameters.Add(new SqlParameter("reservation_is_active", reservation.IsActive));


                return (int)command.ExecuteScalar();
            }
        }

        public List<Reservation> Load()
        {
            var reservations = new List<Reservation>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[reservation]";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "reservation");

                foreach (DataRow row in dataSet.Tables["reservation"]!.Rows)
                {
                    ReservationType reservationType;
                    if (Enum.TryParse(typeof(ReservationType), row["reservation_type"].ToString(), out object parsedType))
                    {
                        reservationType = (ReservationType)parsedType;
                    }
                    else
                    {
                        reservationType = ReservationType.Day;
                    }

                    var reservation = new Reservation()
                    {
                        Id = (int)row["reservation_id"],
                        RoomId = (int)row["reservation_room_id"],
                        ReservationType = reservationType,
                        TotalPrice = (double)row["reservation_total_price"],
                        IsActive = (bool)row["reservation_is_active"]
                    };

                    string[] guestIDs = (row["reservation_guests"] as string).Split('|');
                    reservation.Guests = new List<Guest>();
                    foreach (var guestID in guestIDs)
                    {
                        int id = int.Parse(guestID);
                        Guest guest = GetGuestById(id);
                        if (guest != null)
                        {
                            reservation.Guests.Add(guest);
                        }
                    }


                    string startDateTimeString = row["reservation_start_date"].ToString();
                    string endDateTimeString = row["reservation_end_date"].ToString();

                    Console.WriteLine($"Start Date String: {startDateTimeString}");
                    Console.WriteLine($"End Date String: {endDateTimeString}");

                    DateTime startDateTime;
                    DateTime endDateTime;

                    if (DateTime.TryParseExact(
                        startDateTimeString,
                        "d.MM.yyyy. HH:mm:ss", 
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out startDateTime)
                        && DateTime.TryParseExact(
                            endDateTimeString,
                            "d.MM.yyyy. HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out endDateTime))
                    {
                        reservation.StartDateTime = startDateTime;
                        reservation.EndDateTime = endDateTime;
                    }
                    else
                    {
                       

                        reservation.StartDateTime = DateTime.MinValue;
                        reservation.EndDateTime = DateTime.MinValue;
                    }


                    reservations.Add(reservation);
                }
            }

            return reservations;
        }



        public void Save(List<Reservation> reservationList)
        {
            foreach(Reservation reservation in reservationList)
            {
                if (Exists(reservation.Id))
                {
                    Update(reservation);
                   
                }
                else
                {
                    Insert(reservation);
                }
            }
        }

        public bool Exists(int reservationId)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [dbo].[reservation] WHERE reservation_id = @reservation_id";
                command.Parameters.AddWithValue("@reservation_id", reservationId);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public void Update(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
                    UPDATE [dbo].[reservation]
                    SET reservation_room_id=@reservation_room_id, reservation_type=@reservation_type, reservation_guests=@reservation_guests, reservation_start_date=@reservation_start_date, reservation_end_date=@reservation_end_date, reservation_total_price=@reservation_total_price, reservation_is_active=@reservation_is_active
                    WHERE reservation_id=@reservation_id
                "
                ;
                command.Parameters.Add(new SqlParameter("reservation_id", reservation.Id));
                command.Parameters.Add(new SqlParameter("reservation_room_id", reservation.RoomId));
                command.Parameters.Add(new SqlParameter("reservation_type", reservation.ReservationType.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_guests", string.Join("|", reservation.Guests.Select(guest => guest.Id))));
                command.Parameters.Add(new SqlParameter("reservation_start_date", reservation.StartDateTime.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_end_date", reservation.EndDateTime.ToString()));
                command.Parameters.Add(new SqlParameter("reservation_total_price", reservation.TotalPrice));
                command.Parameters.Add(new SqlParameter("reservation_is_active", reservation.IsActive));

                command.ExecuteNonQuery();
            }
        }

        private Guest GetGuestById(int id)
        {

            return Hotel.GetInstance().Guests.Find(g => g.Id == id)!;
        }
    }
}
