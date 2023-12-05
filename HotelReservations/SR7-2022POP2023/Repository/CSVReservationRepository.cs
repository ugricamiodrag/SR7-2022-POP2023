using HotelReservations.Model;
using HotelReservations.Repository;
using System.Collections.Generic;
using System;
using System.Linq;
using HotelReservations.Exceptions;
using System.IO;

public class CSVReservationRepository : IReservationRepository
{
    private string ToCSV(Reservation reservation)
    {
        string guestIDs = string.Join("|", reservation.Guests.Select(guest => guest.Id));
        return $"{reservation.Id},{reservation.RoomId},{reservation.ReservationType},{guestIDs},{reservation.StartDateTime},{reservation.EndDateTime},{reservation.TotalPrice},{reservation.IsActive}";
    }

    private Reservation FromCSV(string csv)
    {
        string[] parts = csv.Split(',');
        Reservation reservation = new Reservation();
        reservation.Id = int.Parse(parts[0]);
        reservation.RoomId = int.Parse(parts[1]);
        reservation.ReservationType = (ReservationType)Enum.Parse(typeof(ReservationType), parts[2]);

        string[] guestIDs = parts[3].Split('|');
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

        reservation.StartDateTime = DateTime.Parse(parts[4]);
        reservation.EndDateTime = DateTime.Parse(parts[5]);
        reservation.TotalPrice = double.Parse(parts[6]);
        reservation.IsActive = bool.Parse(parts[7]);

        return reservation;
    }

    public List<Reservation> Load()
    {
        if (!File.Exists("reservations.txt"))
        {
            return null!;
        }

        try
        {
            using (var streamReader = new StreamReader("reservations.txt"))
            {
                List<Reservation> resList = new List<Reservation>();
                string line;

                while ((line = streamReader.ReadLine()!) != null)
                {
                    var res = FromCSV(line);
                    resList.Add(res);
                }

                return resList;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new CouldntLoadResourceException(ex.Message);
        }
    }

    public void Save(List<Reservation> reservationList)
    {
        try
        {
            using (var streamWriter = new StreamWriter("reservations.txt"))
            {
                foreach (var res in reservationList)
                {
                    streamWriter.WriteLine(ToCSV(res));
                }
            }
        }
        catch (Exception ex)
        {
            throw new CouldntPersistDataException(ex.Message);
        }
    }


    private Guest GetGuestById(int id)
    {

        return Hotel.GetInstance().Guests.Find(g => g.Id == id)!;
    }

    public int Insert(Reservation reservation)
    {
        throw new NotImplementedException();
    }

    public void Update(Reservation reservation)
    {
        throw new NotImplementedException();
    }
}
