# PLC booking interface - interface between Microsoft Bookings and TalTech Programmable Logic Controllers' lab

## Description

* An interface to track currently booked Programmable Logic Controllers (PLCs) and upcoming bookings
* UI language: English
* Development year: **2024**
* Languages and technologies: **Backend: C#, .NET framework & SQLite  Frontend: HTML, CSS (vanilla) & JavaScript (vanilla)**

## How to run

* The app can be started via terminal/cmd by executing "dotnet run" command in source folder.
* It does not need internet connection to run (as it starts on localhost), but for full functionality, the connection is required to communicate with Microsoft Power Automate and Microsoft Bookings.

### Prerequisites

* .NET SDK (version 8.0 or later)
* SQLite3 (for local database setup)
* Modern web browser (for UI testing)

## Explanation of the structure

### Frontend/UI
The software has two UIs:
*  **Main UI (accessed from `index.html`)**: For users to look for upcoming bookings on the present date. This UI is responsive and can be used on both mobile phones and PCs.
*  **TV UI (accessed from `tv-view.html`)**:  For classrooms' big screens to view currently (exact moment) (un)available PLCs. This UI is suitable only for bigger screens and displays a QR code for students to book a PLC using a smartphone.


### Structure of the backend
The interface's backend consists of three main parts: API controller, business logic layer and data access layer.

* **API controller** - API requests handling.

* **Business logic layer** - validation and data analysis.

* **Data access layer** - communication with SQLite database.

### Data transfer objects (DTOs)
DTOs are used for communicating with Power Automate (via HTTP) and SQLite database

* **Request:**

```csharp
public class Request
{
    public DateTime requestTimestamp { get; set; }
    public string? bookingId { get; set; }
    public DateTime bookingStart { get; set; }
    public DateTime bookingEnd { get; set; }
    public string? plcValue { get; set; }
    public string? requestBody { get; set; }
}
```
  
* **RuleEntry:**

```csharp
public class RuleEntry
{
    public string DayOfWeek { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string PlcIds { get; set; }

    public RuleEntry(string dayOfWeek, string startTime, string endTime, string plcIds)
    {
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        PlcIds = plcIds;
    }
}
```

### Data management
The application uses SQLite database and it stores data locally.

Data is separated between 2 tables. Each table has its own use.

* **UL_PLC_DICTIONARY** - stores all PLC data:

```sql
CREATE TABLE "UL_PLC_DICTIONARY" (
	"plc_id"	INTEGER NOT NULL UNIQUE,
	"plc_value"	TEXT NOT NULL UNIQUE,
	"plc_name"	TEXT NOT NULL UNIQUE,
	"class_PC_ip"	TEXT,
	PRIMARY KEY("plc_id")
);
```
  
* **UL_PLC_BOOKINGS** - stores all bookings:

```sql
CREATE TABLE "UL_PLC_BOOKINGS" (
	"plc_id"	INTEGER NOT NULL,
	"booking_id"	TEXT NOT NULL UNIQUE,
	"start"	INTEGER NOT NULL,
	"end"	INTEGER NOT NULL,
	PRIMARY KEY("booking_id")
);
```

### Rule entry
The interface enables teachers/lecturers to book all (or some of) the PLCs for specific timeslots in repeated pattern (ig. timetable). The feature was requested by our supervisor, because it made him easy to book all PLCs for lectures or lab works.

The interface uses rules to implement this feature. Rules are in rules.txt file and are in format: `!day of week(abbreviation);HH:mm(start);HH:mm(end);<all PLCs to apply the rule to>`.

## Scaling possibilities

### TV UI experience enhancements
TV UI experience can be enhanced with a lot more features (ig. booking reminders)

### API enhancements
API can be developed to send requests to notify users if they are trying to use already occupied PLC without making a reservation.

### iOS/Android application
It is possible to create a phone application which contains UI. Backend still runs on a different server.
