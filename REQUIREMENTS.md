# Growsmart Project Requirements & Setup Guide

## 1. Prerequisites
- **.NET SDK 8.0+**
- **MySQL Server** (Database: `growsmart_db`)
- **ngrok**

## 2. Backend Setup
1. Open `GrowsmartAPI/GrowsmartAPI/appsettings.json` and update your MySQL password and Gemini API Key.
2. The API is now forced to run on **Port 5050** to match ngrok.
3. Run `dotnet run` in the backend folder.

## 3. App Setup
- Start ngrok: `ngrok http 5050`
- The Android app points to the ngrok URL.

## 4. Troubleshooting
- **502 Bad Gateway**: Ensure the backend terminal says it is listening on port **5050**. If it says 5000, restart the backend.
