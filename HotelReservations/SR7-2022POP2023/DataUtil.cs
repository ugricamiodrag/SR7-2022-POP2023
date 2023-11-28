using HotelReservations.Exceptions;
using HotelReservations.Model;
using HotelReservations.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservations
{
    public class DataUtil
    {
        public static void LoadData()
        {
            Hotel hotel = Hotel.GetInstance();
            hotel.Id = 1;
            hotel.Name = "Hotel Park";
            hotel.Address = "Kod Futoskog parka...";

            try
            {
                IRoomRepository roomRepository = new RoomRepository();
                List<RoomType> loadedRoomTypes = roomRepository.GetRoomTypes();

                if (loadedRoomTypes != null)
                {

                    Hotel.GetInstance().RoomTypes = loadedRoomTypes;

                    roomRepository.SaveRT(loadedRoomTypes);


                    var loadedRooms = roomRepository.Load();
                    IGuestRepository guestRepository = new GuestRepository();
                    var guests = guestRepository.Load();
                    IUserRepository userRepository = new UserRepository();
                    var loadedUsers = userRepository.Load();
                    IPriceListRepository priceListRepository = new PriceListRepository();
                    var loadedPrices = priceListRepository.Load();
                    IReservationRepository reservationRepository = new ReservationRepository();
                    var loadedReservation = reservationRepository.Load();



                    if (loadedRooms != null)
                    {
                        Hotel.GetInstance().Rooms = loadedRooms;
                    }

                    if (guests != null)
                    {
                        Hotel.GetInstance().Guests = guests;
                    }

                    if (loadedUsers != null)
                    {
                        Hotel.GetInstance().Users = loadedUsers;
                    }

                    if (loadedPrices != null)
                    {
                        Hotel.GetInstance().PriceList = loadedPrices;
                    }

                    if (loadedReservation != null)
                    {
                        Hotel.GetInstance().Reservations = loadedReservation;
                    }

                }
                else
                {
                    Console.WriteLine("No room types found.");
                }
            }
            catch (CouldntLoadResourceException)
            {
                Console.WriteLine("Call an administrator, something weird is happening with the files");
            }
            catch (Exception ex)
            {
                Console.Write("An unexpected error occurred", ex.Message);
            }
        }


        public static void PersistData()
        {
            try
            {
                // Kada se gasi program, čuvamo u rooms.txt
                // Posle toga će rooms.txt postojati (ako nešto ne pođe po zlu)



                IRoomRepository roomRepository = new RoomRepository();
                roomRepository.SaveRT(Hotel.GetInstance().RoomTypes);
                roomRepository.Save(Hotel.GetInstance().Rooms);

                IGuestRepository guestRepository = new GuestRepository();
                guestRepository.Save(Hotel.GetInstance().Guests);

                IUserRepository userRepository = new UserRepository();
                userRepository.Save(Hotel.GetInstance().Users);

                IPriceListRepository priceListRepository = new PriceListRepository();
                priceListRepository.Save(Hotel.GetInstance().PriceList);

                IReservationRepository reservationRepository = new ReservationRepository();
                reservationRepository.Save(Hotel.GetInstance().Reservations);


                //BinaryRoomRepository binaryRoomRepository = new BinaryRoomRepository();
                //binaryRoomRepository.Save(Hotel.GetInstance().Rooms);

            }
            catch (CouldntPersistDataException)
            {
                Console.WriteLine("Call an administrator, something weird is happening with the files");
            }
        }
    }
}
