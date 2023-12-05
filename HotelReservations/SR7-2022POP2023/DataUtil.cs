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
                IRoomTypeRepository roomTypeRepository = new RoomTypeRepository();
                List<RoomType> loadedRoomTypes = roomTypeRepository.Load();

                if (loadedRoomTypes != null)
                {

                    Hotel.GetInstance().RoomTypes = loadedRoomTypes;

                    roomTypeRepository.Save(loadedRoomTypes);
                }
                else
                {
                    Console.WriteLine("No room types found.");
                }

                IRoomRepository roomRepository = new RoomRepository();
                var loadedRooms = roomRepository.Load();
                if (loadedRooms != null)
                {
                    Hotel.GetInstance().Rooms = loadedRooms;
                    roomRepository.Save(loadedRooms);
                }
                IGuestRepository guestRepository = new GuestRepository();
                var guests = guestRepository.Load();

                if (guests != null)
                {
                    Hotel.GetInstance().Guests = guests;
                    guestRepository.Save(guests);
                }

                IUserRepository userRepository = new UserRepository();
                var loadedUsers = userRepository.Load();
                if (loadedUsers != null)
                {
                    Hotel.GetInstance().Users = loadedUsers;
                    userRepository.Save(loadedUsers);
                }


                IPriceListRepository priceListRepository = new PriceListRepository();
                var loadedPrices = priceListRepository.Load();
                if (loadedPrices != null)
                {
                    Hotel.GetInstance().PriceList = loadedPrices;
                    priceListRepository.Save(loadedPrices);
                }


                IReservationRepository reservationRepository = new ReservationRepository();
                var loadedReservation = reservationRepository.Load();

                if ( loadedReservation != null )
                {
                    Hotel.GetInstance().Reservations = loadedReservation;
                    reservationRepository.Save(loadedReservation);
                }



                //if (loadedRooms != null)
                //{
                //    Hotel.GetInstance().Rooms = loadedRooms;
                //}

                //if (guests != null)
                //{
                //    Hotel.GetInstance().Guests = guests;
                //}

                //if (loadedUsers != null)
                //{
                //    Hotel.GetInstance().Users = loadedUsers;
                //}

                //if (loadedPrices != null)
                //{
                //    Hotel.GetInstance().PriceList = loadedPrices;
                //}

                //if (loadedReservation != null)
                //{
                //    Hotel.GetInstance().Reservations = loadedReservation;
                //}
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
                IRoomTypeRepository roomTypeRepository = new RoomTypeRepository();
                roomTypeRepository.Save(Hotel.GetInstance().RoomTypes);
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
