using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using HotelReservations.Model;
using HotelReservations.Service;

namespace HotelReservations.Windows
{
    public partial class CheckOutGuestsHandler
    {
        private ReservationService reservationService = new ReservationService();
        private RoomService roomService = new RoomService();
        private PriceListService priceListService = new PriceListService();
        private Reservation selectedReservation;

        public CheckOutGuestsHandler(Reservation selectedReservation)
        {
            this.selectedReservation = selectedReservation;
            CheckOutSelectedReservation();
        }

        public List<Room> GetOccupiedRooms()
        {
            return reservationService.GetOccupiedRoomsWithoutEndDateAndPrice();
        }

        public List<Reservation> GetReservationsForRoom(int roomId)
        {
            return reservationService.GetReservationsForRoom(roomId);
        }

        public void CheckOutSelectedReservation()
        {
            if (selectedReservation != null && selectedReservation.EndDateTime == null)
            {
                DateTime endDate = DateTime.Now;
                string endDateTimeString = endDate.ToString("dd.MM.yyyy. HH:mm:ss"); // Replace with your actual string

                if (!string.IsNullOrEmpty(endDateTimeString) && DateTime.TryParseExact(
                   endDateTimeString,
                    "dd.MM.yyyy. HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out endDate))
                {
                    if (endDate > selectedReservation.StartDateTime)
                    {
                        var roomType = roomService.GetRoomTypeById(selectedReservation.RoomId);
                        var difference = (int)(endDate - selectedReservation.StartDateTime).TotalDays;
                        ReservationType resType = difference > 1 ? ReservationType.Night : ReservationType.Day;
                        List<Price> prices = GetPricesForRoomType(roomType, resType);

                        if (prices.Any())
                        {
                            double priceValue = prices.FirstOrDefault()?.PriceValue ?? 0;
                            double totalPrice = priceValue * difference;

                            selectedReservation.TotalPrice = totalPrice;
                            selectedReservation.EndDateTime = endDate;

                            reservationService.SaveReservation(selectedReservation);

                            MessageBox.Show($"Checked out Reservation ID: {selectedReservation.Id}");

                        }
                        else
                        {
                            MessageBox.Show("No valid prices found for this room type and reservation type.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("End date must be after the start date.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to parse EndDateTime string.");
                }
            }
            else if (selectedReservation != null && selectedReservation.EndDateTime != null)
            {
                MessageBox.Show("This reservation is already checked out.");
            }
            else
            {
                MessageBox.Show("Reservation data is empty.");
            }
        }

        private List<Price> GetPricesForRoomType(RoomType roomType, ReservationType reservationType)
        {
            List<Price> pricesList = priceListService.GetAllPrices();
            var pricesForRoom = pricesList.Where(price =>
                price.RoomType.Name == roomType.Name &&
                price.ReservationType == reservationType &&
                price.IsActive == true).ToList();

            return pricesForRoom;
        }

    }
}
