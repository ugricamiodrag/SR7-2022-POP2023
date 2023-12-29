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
                    ";

                    command.Parameters.Add(new SqlParameter("@reservation_room_id", reservation.RoomId));
                    command.Parameters.Add(new SqlParameter("@reservation_type", reservation.ReservationType.ToString()));
                    command.Parameters.Add(new SqlParameter("@reservation_start_date", reservation.StartDateTime.ToString()));
                    command.Parameters.Add(new SqlParameter("@reservation_end_date", reservation.EndDateTime != null ? (object)reservation.EndDateTime.ToString() : DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@reservation_total_price", reservation.TotalPrice != null ? (object)reservation.TotalPrice : DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@reservation_is_active", reservation.IsActive));


                foreach (var guest in reservation.Guests)
                {
                    AddGuestToRoom(guest.Id, reservation.RoomId, reservation.StartDateTime, reservation.EndDateTime);
                }

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
                        TotalPrice = row["reservation_total_price"] != DBNull.Value ? (double)row["reservation_total_price"] : default(double),
                        IsActive = (bool)row["reservation_is_active"]
                    };

                    string startDateTimeString = row["reservation_start_date"].ToString();
                    string endDateTimeString = row["reservation_end_date"].ToString();

                    DateTime startDateTime;
                    DateTime? endDateTime = null;

                    if (!string.IsNullOrEmpty(startDateTimeString) && DateTime.TryParseExact(
                            startDateTimeString,
                            "dd.MM.yyyy. HH:mm:ss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out startDateTime))
                    {
                        reservation.StartDateTime = startDateTime;

                        if (!string.IsNullOrEmpty(endDateTimeString) && DateTime.TryParseExact(
                                endDateTimeString,
                                "dd.MM.yyyy. HH:mm:ss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out var tempEndDateTime))
                        {
                            endDateTime = tempEndDateTime;
                        }

                        reservation.EndDateTime = endDateTime;
                    }
                    else
                    {
                        reservation.StartDateTime = DateTime.MinValue;
                        reservation.EndDateTime = null;
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
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                   
                    UpdateReservation(conn, transaction, reservation);

                    UpdateGuests(conn, transaction, reservation);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private void UpdateReservation(SqlConnection conn, SqlTransaction transaction, Reservation reservation)
        {
            var command = conn.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
        UPDATE [dbo].[reservation]
        SET reservation_room_id = @reservation_room_id, reservation_type = @reservation_type, reservation_start_date = @reservation_start_date, reservation_end_date = @reservation_end_date, reservation_total_price = @reservation_total_price, reservation_is_active = @reservation_is_active
        WHERE reservation_id = @reservation_id
    ";

            command.Parameters.AddWithValue("@reservation_id", reservation.Id);
            command.Parameters.AddWithValue("@reservation_room_id", reservation.RoomId);
            command.Parameters.AddWithValue("@reservation_type", reservation.ReservationType.ToString());
            command.Parameters.AddWithValue("@reservation_start_date", reservation.StartDateTime.ToString("dd.MM.yyyy. HH:mm:ss"));
            command.Parameters.AddWithValue("@reservation_end_date", reservation.EndDateTime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@reservation_total_price", reservation.TotalPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@reservation_is_active", reservation.IsActive);

            command.ExecuteNonQuery();
        }


        private void UpdateGuests(SqlConnection conn, SqlTransaction transaction, Reservation reservation)
        {
            List<Guest> existingGuests = GetGuests(reservation);
            if (existingGuests == null)
            {
                throw new Exception("Failed to retrieve guests for the reservation.");
            }

            List<Guest> newGuests = reservation.Guests;

            List<Guest> guestsToRemove = existingGuests.Except(newGuests).ToList();
            foreach (var guest in guestsToRemove)
            {
                RemoveGuestFromRoom(conn, transaction, reservation.RoomId, guest.Id, reservation.StartDateTime, reservation.EndDateTime);
            }

            foreach (var guest in newGuests)
            {
                AddOrUpdateGuest(conn, transaction, reservation.RoomId, guest.Id, reservation.StartDateTime, reservation.EndDateTime);
            }
        }

        private void RemoveGuestFromRoom(SqlConnection conn, SqlTransaction transaction, int roomId, int guestId, DateTime startDate, DateTime? endDate)
        {
            var command = conn.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM [dbo].[GuestsInRoom] WHERE room_id = @room_id AND startDate = @startDate AND endDate = @endDate AND guest_id = @guest_id";

            command.Parameters.AddWithValue("@room_id", roomId);
            command.Parameters.AddWithValue("@startDate", startDate.ToString("dd.MM.yyyy. HH:mm:ss"));
            command.Parameters.AddWithValue("@endDate", endDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@guest_id", guestId);

            command.ExecuteNonQuery();
        }

        private void AddOrUpdateGuest(SqlConnection conn, SqlTransaction transaction, int roomId, int guestId, DateTime startDate, DateTime? endDate)
        {
            var command = conn.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = "SELECT COUNT(*) FROM [dbo].[GuestsInRoom] WHERE room_id = @room_id AND startDate = @startDate AND guest_id = @guest_id";
            command.Parameters.AddWithValue("@room_id", roomId);
            command.Parameters.AddWithValue("@startDate", startDate.ToString("dd.MM.yyyy. HH:mm:ss"));
            command.Parameters.AddWithValue("@guest_id", guestId);

            int count = (int)command.ExecuteScalar();

            if (count > 0)
            {
                
                command.CommandText = "UPDATE [dbo].[GuestsInRoom] SET endDate = @endDate WHERE room_id = @room_id AND startDate = @startDate AND guest_id = @guest_id";
                command.Parameters.Clear(); 
                command.Parameters.AddWithValue("@endDate", endDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@room_id", roomId);
                command.Parameters.AddWithValue("@startDate", startDate.ToString("dd.MM.yyyy. HH:mm:ss"));
                command.Parameters.AddWithValue("@guest_id", guestId);
            }
            else
            {
           
                command.CommandText = "INSERT INTO [dbo].[GuestsInRoom] (room_id, startDate, endDate, guest_id) VALUES (@room_id, @startDate, @endDate, @guest_id)";
                command.Parameters.Clear(); 
                command.Parameters.AddWithValue("@room_id", roomId);
                command.Parameters.AddWithValue("@startDate", startDate.ToString("dd.MM.yyyy. HH:mm:ss"));
                command.Parameters.AddWithValue("@endDate", endDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@guest_id", guestId);
            }

            command.ExecuteNonQuery();
        }


        public List<Guest> GetGuests(Reservation reservation)
        {
            List<Guest> guests = new List<Guest>();
            using (SqlConnection conn = new SqlConnection(Config.CONNECTION_STRING))
            {
                var commandText = "SELECT * FROM [dbo].[GuestsInRoom] WHERE room_id = @room_id AND startDate = @startDate";
                SqlCommand command = new SqlCommand(commandText, conn);
                command.Parameters.AddWithValue("@room_id", reservation.RoomId);
                command.Parameters.AddWithValue("@startDate", reservation.StartDateTime.ToString("dd.MM.yyyy. HH:mm:ss"));

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();

                adapter.Fill(dataSet, "GuestsInRoom");

                foreach (DataRow row in dataSet.Tables["GuestsInRoom"]!.Rows)
                {
                    int guestId = (int)row["guest_id"];
                    Guest guest = GetGuestById(guestId);
                    if (guest != null)
                    {
                        guests.Add(guest);
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

                var commandCheck = conn.CreateCommand();
                commandCheck.CommandText = "SELECT COUNT(*) FROM [dbo].[GuestsInRoom] WHERE guest_id = @guest_id AND room_id = @room_id";
                commandCheck.Parameters.AddWithValue("@guest_id", guestId);
                commandCheck.Parameters.AddWithValue("@room_id", roomId);

                int count = (int)commandCheck.ExecuteScalar();

                if (count == 0)
                {
                    var command = conn.CreateCommand();
                    command.CommandText = @"
                INSERT INTO [dbo].[GuestsInRoom] (guest_id, room_id, startDate, endDate)
                VALUES (@guest_id, @room_id, @startDate, @endDate)
            ";

                    command.Parameters.Add(new SqlParameter("guest_id", guestId));
                    command.Parameters.Add(new SqlParameter("room_id", roomId));
                    command.Parameters.Add(new SqlParameter("startDate", startDate.ToString("dd.MM.yyyy. HH:mm:ss")));
                    command.Parameters.Add(new SqlParameter("endDate", endDate.HasValue ? endDate.Value.ToString("dd.MM.yyyy. HH:mm:ss") : DBNull.Value));

                    command.ExecuteNonQuery();
                }
                
            }
        }


    }
}
