# Project: Hotel Management System

This .NET WPF application provides a desktop solution for managing hotel operations, including room administration, room rentals, pricing, and user access. The application is restricted to authenticated users and supports two roles: administrator and receptionist.

## Technologies

- .NET (WPF)
- Relational database


## Features

### Room Management
- Create, update, view, and logically delete rooms
- Each room includes:
  - Room number
  - Room type (number of beds)
  - Has TV (yes/no)
  - Has mini-bar (yes/no)

### Room Rentals
- Rental types:
  - Overnight stay
  - Day stay
- Tracks:
  - First and last name, and ID number for each guest
  - Start and end date/time of the rental
  - Total rental cost (calculated automatically at checkout based on pricing)

### Pricing Management
- Define prices per room type:
  - Overnight price
  - Day-stay price

### User Management
- Stores:
  - First and last name
  - Unique ID (JMBG)
  - Username and password
  - User role: administrator or receptionist
- Only authenticated users can access the application
- Only administrators can manage users, room listings, and pricing
- Receptionists manage room rentals

## Search and Sorting
- All entity lists support column-based sorting
- Filtering options:
  - **Rooms:** by type and status (available/occupied)
  - **Rentals:** by room number, check-in and check-out dates
  - **Users:** by username
  - **Active Rentals:** shows currently occupied rooms and guests

## Pricing Calculation
- Upon checkout, the application automatically calculates the amount owed based on the defined pricing and duration of the stay

## Persistence
- All data is persisted using a relational database
- Deletions are logical (entities are marked as inactive, not physically removed)

## Note
This application is developed as a student project for the course **Object-Oriented Programming Platforms** (2023/2024 academic year) and is not intended for commercial use.
