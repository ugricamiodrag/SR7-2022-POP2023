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
                    INSERT INTO [dbo].[reservation] (reservation_room_id, reservation_type, reservation_start_date, reservation_end_date, reservation_total_price, reservation_is_active)
                    OUTPUT inserted.reservation_id
                    VALUES (@reservation_room_id, @reservation_type, @reservation_start_date, @reservation_end_date, @reservation_total_price, @reservation_is_active)
                "
                ;

                command.Parameters.Add(new SqlParameter("reservation_room_id", reservation.RoomId));
                command.Parameters.Add(new SqlParameter("reservation_type", reservation.ReservationType.ToString()));
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

                    


                    string startDateTimeString = row["reservation_start_date"].ToString();
                    string endDateTimeString = row["reservation_end_date"].ToString();

                    Console.WriteLine($"Start Date String: {startDateTimeString}");
                    Console.WriteLine($"End Date String: {endDateTimeString}");

                    DateTime startDateTime;
                    DateTime endDateTime;

                    if (DateTime.TryParseExact(
                        startDateTimeString,
                        "d.MM.yyyy.", 
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out startDateTime)
                        && DateTime.TryParseExact(
                            endDateTimeString,
                            "d.MM.yyyy.",
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

                    reservation.Guests = GetGuests(reservation);


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

                // Begin transaction
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    var command = conn.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE [dbo].[reservation]
                        SET reservation_room_id=@reservation_room_id, reservation_type=@reservation_type, reservation_start_date=@reservation_start_date, reservation_end_date=@reservation_end_date, reservation_total_price=@reservation_total_price, reservation_is_active=@reservation_is_active
                        WHERE reservation_id=@reservation_id
                             ";
                    command.Parameters.Add(new SqlParameter("reservation_id", reservation.Id));
                    command.Parameters.Add(new SqlParameter("reservation_room_id", reservation.RoomId));
                    command.Parameters.Add(new SqlParameter("reservation_type", reservation.ReservationType.ToString()));
                    command.Parameters.Add(new SqlParameter("reservation_start_date", reservation.StartDateTime.ToString()));
                    command.Parameters.Add(new SqlParameter("reservation_end_date", reservation.EndDateTime.ToString()));
                    command.Parameters.Add(new SqlParameter("reservation_total_price", reservation.TotalPrice));
                    command.Parameters.Add(new SqlParameter("reservation_is_active", reservation.IsActive));

                    command.ExecuteNonQuery();
                    List<Guest> oldGuests = GetGuests(reservation);
                    if (oldGuests == null)
                    {
                        throw new Exception("Failed to retrieve guests for the reservation.");
                    }

                
                    List<Guest> newGuests = reservation.Guests;

        
                    List<Guest> guestsToRemove = oldGuests.Except(newGuests).ToList();

                    List<Guest> guestsToAdd = newGuests.Except(oldGuests).ToList();

                    if (guestsToRemove.Any())
                    {
                        command.CommandText = "DELETE FROM [dbo].[GuestsInRoom] WHERE room_id=@room_id AND startDate=@startDate AND endDate=@endDate AND guest_id=@guest_id";
                        command.Parameters.Clear();
                        foreach (var guest in guestsToRemove)
                        {
                            command.Parameters.AddWithValue("@room_id", reservation.RoomId);
                            command.Parameters.AddWithValue("@startDate", reservation.StartDateTime);
                            command.Parameters.AddWithValue("@endDate", reservation.EndDateTime);
                            command.Parameters.AddWithValue("@guest_id", guest.Id);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }


                    if (guestsToAdd.Any())
                    {
                        command.CommandText = "INSERT INTO [dbo].[GuestsInRoom] (room_id, startDate, endDate, guest_id) VALUES (@room_id, @startDate, @endDate, @guest_id)";
                        command.Parameters.Clear();
                        foreach (var guest in guestsToAdd)
                        {
                            command.Parameters.AddWithValue("@room_id", reservation.RoomId);
                            command.Parameters.AddWithValue("@startDate", reservation.StartDateTime);
                            command.Parameters.AddWithValue("@endDate", reservation.EndDateTime);
                            command.Parameters.AddWithValue("@guest_id", guest.Id);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

            public List<Guest> GetGuests(Reservation reservation)
        {
            List<Guest> guests = new List<Guest>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[GuestsInRoom]";
                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);

                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "guests");

                foreach (DataRow row in dataSet.Tables["guests"]!.Rows)
                {
                    int guestId = (int)row["guest_id"];
                    int roomId = (int)row["room_id"];
                    DateTime startDate = DateTime.Parse(row["startDate"].ToString());
                    DateTime? endDate = row["endDate"] != DBNull.Value ? DateTime.Parse(row["endDate"].ToString()) : (DateTime?)null;

                    if (roomId == reservation.RoomId && startDate == reservation.StartDateTime && endDate == reservation.EndDateTime)
                    {
                        Guest guest = GetGuestById(guestId);
                        if (guest != null)
                        {
                            guests.Add(guest);
                        }
                    }
                }
            }
            return guests;
        }


        private Guest GetGuestById(int id)
        {

            return Hotel.GetInstance().Guests.Find(g => g.Id == id)!;
        }

        public void AddGuestToRoom(int guestId, int roomId, DateTime startDate, DateTime? endDate)
        {
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                conn.Open();

                var command = conn.CreateCommand();
                command.CommandText = @"
            INSERT INTO [dbo].[GuestsInRoom] (guest_id, room_id, startDate, endDate)
            VALUES (@guest_id, @room_id, @startDate, @endDate)
        ";

                command.Parameters.Add(new SqlParameter("guest_id", guestId));
                command.Parameters.Add(new SqlParameter("room_id", roomId));
                command.Parameters.Add(new SqlParameter("startDate", startDate.ToString()));
                command.Parameters.Add(new SqlParameter("endDate", endDate.HasValue ? endDate.Value.ToString() : DBNull.Value));

                command.ExecuteNonQuery();
            }
        }

    }
}
