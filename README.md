# PLC booking interface - interface between Microsoft Bookings and TalTech Programmable Logic Controllers' lab

## Description

* An interface to track currently booked Programmable Logic Controllers (PLCs) and upcoming bookings
* UI language: English
* Development year: **2024**
* Languages and technologies: **Backend: C#, .NET framework & SQLite  Frontend: HTML, CSS (vanilla) & JavaScript (vanilla)**

## How to run

* The app can be started via terminal/cmd by executing "dotnet run" command in source folder.
* It does not need internet connection to run (as it starts on localhost), but for full functionality, the connection is required to communicate with Microsoft Power Automate and Microsoft Bookings.

## Explanation of the structure

### Frontend/UI
The software has two UIs:

* Main UI (accessed from index.html):  for users to look for upcoming bookings on present date. This UI can be used both on mobile phones and on PCs (design is responsive).
* TV UI (accessed from tv-view.html): for classrooms' big screens to view currently (exact moment) (un)available PLCs. This UI is suitable only for bigger screens and has a QR code for students to book a PLC using smartphone.

### Structure of the backend
The interface's backend consists of three main parts: API controller, business logic layer and data access layer.

* **API controller** - API requests handling.

* **Business logic layer** - validation and data analysis.

* **Data access layer** - communication with SQLite database.

### Data transfer objects (DTOs)
DTOs are used for communicating with Power Automate (via HTTP) and SQLite database

* **Request** - HTTP request data (consists of: requestTimestamp (datetime format), bookingId(string format: generated by MS bookings), bookingStart (datetime format), bookingEnd (datetime format), plcValue (string format: generated by MS bookings), requestBody (string format - action).
  
* **RuleEntry** - rule data (consists of: DayOfWeek(string format: abbreviation of day of week), StartTime (datetime format) , EndTime (datetime format), PlcIds (string format: all affected PLC IDs separated by comma)).

### Data management
The application uses SQLite database and it stores data locally.

Data is separated between 2 tables. Each table has its own use.

* **UL_PLC_DICTIONARY** - stores all PLC data (plc_id, plc_value, plc_name).
  
* **UL_PLC_BOOKINGS** - stores all bookings (plc_id, booking_id, start, end).

### Rule entry
The interface enables teachers/lecturers to book all (or some of) the PLCs for specific timeslots in repeated pattern (ig. timetable). The feature was requested by our supervisor, because it made him easy to book all PLCs for lectures or lab works.

The interface uses rules to implement this feature. Rules are in rules.txt file and are in format: "!day of week(abbreviation);HH:mm(start);HH:mm(end);<all PLCs to apply the rule to>".

## Scaling possibilities

### TV UI experience enhancements
TV UI experience can be enhanced with a lot more features (ig. booking reminders)

### API enhancements
API can be developed to send requests to notify users if they are trying to use already occupied PLC without making a reservation.

### iOS/Android application
It is possible to create a phone application which contains UI. Backend still runs on a different server.