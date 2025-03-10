# ResourceManagerApp

## Project Description

ResourceManagerApp is a backend application designed for managing an online resource reservation. The project allows users to browse, add, and manage reservations of fictional office's rooms.

## Technologies

This project is built using the following technologies:

- **Backend:** C# (.NET Core)
- **Authentication:** JWT and login via Google
- **Database:** PostgreSQL

## Installation and Running

1. **Clone the repository:**
   ```sh
   git clone https://github.com/Neeyo1/ResourceManagerApp.git
   cd ResourceManagerApp/API
   ```
2. **Run the application using .NET CLI:**
   ```sh
   dotnet run
   ```

# API Documentation

## Overview
This API provides authentication, user account management, room reservations, and room management functionalities.

**Swagger docs:**
`https://localhost:5001/swagger/index.html`

## Authentication

### Google Login
**Endpoint:**
```
GET /api/Auth/google-login
```
**Description:** Initiates Google authentication.

**Response:**
- `200 OK` - Success

### Google Response
**Endpoint:**
```
GET /api/Auth/google-response
```
**Description:** Handles response from Google authentication.

**Response:**
- `200 OK` - Success

---

## User Account Management

### Login
**Endpoint:**
```
POST /api/Account/login
```
**Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```
**Response:**
- `200 OK` - Returns user data with JWT token.

### Register
**Endpoint:**
```
POST /api/Account/register
```
**Request Body:**
```json
{
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "password": "string"
}
```
**Response:**
- `200 OK` - Returns user data with JWT token.

### Change Password
**Endpoint:**
```
POST /api/Account/change-password
```
**Request Body:**
```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```
**Response:**
- `200 OK` - Password updated successfully.

### Refresh Token
**Endpoint:**
```
POST /api/Account/refresh
```
**Request Body:**
```json
{
  "token": "string"
}
```
**Response:**
- `200 OK` - Returns a new JWT token.

---

## Room Reservations

### Get Available Rooms
**Endpoint:**
```
GET /api/Reservations/rooms
```
**Query Parameters:**
- `ReservedFrom`: DateTime
- `ReservedTo`: DateTime
- `RoomId`: Integer
- `UserId`: Integer
- `Status`: String
- `OrderBy`: String
- `PageNumber`: Integer
- `PageSize`: Integer

**Response:**
- `200 OK` - Returns list of available rooms.

### Reserve a Room
**Endpoint:**
```
POST /api/Reservations/rooms
```
**Request Body:**
```json
{
  "reservedFrom": "datetime",
  "reservedTo": "datetime",
  "roomId": 1
}
```
**Response:**
- `200 OK` - Room reservation successful.

### Get Reservation Details
**Endpoint:**
```
GET /api/Reservations/rooms/{roomReservationId}
```
**Response:**
- `200 OK` - Returns reservation details.

### Update Reservation
**Endpoint:**
```
PUT /api/Reservations/rooms/{roomReservationId}
```
**Request Body:**
```json
{
  "reservedFrom": "datetime",
  "reservedTo": "datetime"
}
```
**Response:**
- `200 OK` - Reservation updated successfully.

### Cancel Reservation
**Endpoint:**
```
DELETE /api/Reservations/rooms/{roomReservationId}
```
**Response:**
- `200 OK` - Reservation cancelled successfully.

---

## Room Management

### Get Rooms
**Endpoint:**
```
GET /api/Rooms
```
**Query Parameters:**
- `Name`: String
- `MinCapacity`: Integer
- `MaxCapacity`: Integer
- `Type`: String
- `OrderBy`: String
- `PageNumber`: Integer
- `PageSize`: Integer

**Response:**
- `200 OK` - Returns list of rooms.

### Create Room
**Endpoint:**
```
POST /api/Rooms
```
**Request Body:**
```json
{
  "name": "string",
  "capacity": 10,
  "roomType": "string"
}
```
**Response:**
- `200 OK` - Room created successfully.

### Get Room Details
**Endpoint:**
```
GET /api/Rooms/{roomId}
```
**Response:**
- `200 OK` - Returns room details.

### Update Room
**Endpoint:**
```
PUT /api/Rooms/{roomId}
```
**Request Body:**
```json
{
  "name": "string",
  "capacity": 15,
  "roomType": "string"
}
```
**Response:**
- `200 OK` - Room updated successfully.

### Delete Room
**Endpoint:**
```
DELETE /api/Rooms/{roomId}
```
**Response:**
- `200 OK` - Room deleted successfully.

### Get Room Status
**Endpoint:**
```
GET /api/Rooms/status
```
**Query Parameters:**
- `dateStart`: String
- `dateEnd`: String

**Response:**
- `200 OK` - Returns room availability.
