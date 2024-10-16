const dateTimeNowCheckbox = document.getElementById("dateTimeNow");
const bookingDateSelectionCalender = document.getElementById("bookingDate");
const timeRange = document.getElementById("timeRange");
const plcUnavailableText = document.getElementById("plc-unavailable-text");
const plcAvailableText = document.getElementById("plc-available-text");

function updateTime() {
    const selectedTimeStartDisplay = document.getElementById("selectedTimeStart");
    const selectedTimeEndDisplay = document.getElementById("selectedTimeEnd");

    const startHour = Math.floor(timeRange.value / 2); 
    const startMinutes = (timeRange.value % 2) * 30; 

    const endValue = Math.min(Number(timeRange.value) + 2, 47); 
    const endHour = Math.floor(endValue / 2);
    const endMinutes = (endValue % 2) * 30; 

    const formattedStartTime = `${String(startHour).padStart(2, '0')}:${String(startMinutes).padStart(2, '0')}`;

    const formattedEndTime = `${String(endHour).padStart(2, '0')}:${String(endMinutes).padStart(2, '0')}`;
    selectedTimeStartDisplay.textContent = formattedStartTime;
    selectedTimeEndDisplay.textContent = formattedEndTime;
}

function toggleDateTimeControls() {
    bookingDateSelectionCalender.disabled = dateTimeNowCheckbox.checked;
    timeRange.disabled = dateTimeNowCheckbox.checked;
}

async function fetchBookedPLCs() {
    deleteRequests;
    const selectedDateInput = document.getElementById("bookingDate").value;
    const startTimeInput = document.getElementById("selectedTimeStart").textContent;
    const endTimeInput = document.getElementById("selectedTimeEnd").textContent;


    const currentDate = new Date();
    const currentDateString = currentDate.toISOString().split("T")[0];
    const currentHour = currentDate.getHours();
    const currentMinutes = Math.floor(currentDate.getMinutes() / 30) * 30;
    const formattedCurrentMinutes = currentMinutes === 60 ? '00' : String(currentMinutes).padStart(2, '0');

    const selectedDate = selectedDateInput || currentDateString;
    const startTime = startTimeInput || `${String(currentHour).padStart(2, '0')}:${formattedCurrentMinutes}`;
    const endTime = endTimeInput || startTime;

    const finalDateTimeStart = `${selectedDate}T${startTime}:00`;
    const finalDateTimeEnd = `${selectedDate}T${endTime}:00`;

    try {
        const response = await fetch(`/api/requests/booked_PLCs?dateTimeStart=${encodeURIComponent(finalDateTimeStart)}&dateTimeEnd=${encodeURIComponent(finalDateTimeEnd)}`);

        if (!response.ok) {
            throw new Error('Failed to fetch booked PLCs');
        }

        const bookedPLCs = await response.json();
        updatePLCStyles(bookedPLCs);
    } catch (error) {
        console.error('Error fetching booked PLCs:', error);
    }
}

function updatePLCStyles(bookedPLCs) {
    const radioButtons = document.querySelectorAll('.radio-input');

    if (bookedPLCs.length === 0) {
        radioButtons.forEach(radio => {
            const radioLabel = radio.nextElementSibling;
            radioLabel.style.border = '5px solid #37A7BD'; 
        });
    } else {
        radioButtons.forEach(radio => {
            const radioLabel = radio.nextElementSibling;
            if (bookedPLCs.includes(parseInt(radio.id))) {
                radioLabel.style.border = '5px solid #BD4D37';
            } else {
                radioLabel.style.border = '5px solid #37A7BD'; 
            }
        });
    }
}

async function deleteRequests() {
    let url = "/api/requests/booked_PLCs";
    try {
        const response = await fetch(url, {
            method: 'DELETE', 
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Failed to delete resource');
        }

        const data = await response.json();
        console.log('Delete successful:', data);
    } catch (error) {
        console.error('Error:', error);
    }
}

async function fetchPLCBookings(plcId) {
    try {
        const response = await fetch(`/api/requests/get_plc_bookings?plcId=${encodeURIComponent(plcId)}`);

        if (!response.ok) {
            throw new Error('Failed to fetch bookings data');
        }

        const bookings = await response.json();
        return bookings;
    } catch (error) {
        console.error('Error fetching PLC bookings:', error);
        return [];
    }
}

function displayBookings(bookings) {
    const plcUnavailableText = document.getElementById("plc-unavailable-text");
    const plcAvailableText = document.getElementById("plc-available-text");

    plcUnavailableText.innerHTML = '';
    plcAvailableText.innerHTML = '';
    const startHour = 0;
    const endHour = 23;
    const numDays = 7;

    const bookedSlots = new Set();

    // Process bookings and populate bookedSlots set
    bookings.forEach(booking => {
        const startTime = new Date(booking.start);
        const endTime = new Date(booking.end);

        let currentTime = new Date(startTime);
        while (currentTime < endTime) {
            bookedSlots.add(currentTime.toISOString());
            currentTime.setMinutes(currentTime.getMinutes() + 30);
        }

        // Create booking display
        const formattedStartTime = `${String(startTime.getHours()).padStart(2, '0')}:${String(startTime.getMinutes()).padStart(2, '0')}`;
        const formattedEndTime = `${String(endTime.getHours()).padStart(2, '0')}:${String(endTime.getMinutes()).padStart(2, '0')}`;
        const bookingText = document.createElement('p');
        bookingText.textContent = `${formattedStartTime} - ${formattedEndTime}`;
        plcUnavailableText.appendChild(bookingText);
    });

    const availableTimes = [];
    const today = new Date();
    today.setHours(startHour, 0, 0, 0);
   for (let day = 0; day < numDays; day++) {
    let currentTime = new Date(today);
    currentTime.setDate(today.getDate() + day);
    currentTime.setHours(startHour, 0, 0, 0);

    // Check if there are any bookings for this day
    let hasBookingsForDay = false;

    // Collect bookings for the current day
    while (currentTime.getHours() < endHour) {
        const timeISO = currentTime.toISOString();
        if (bookedSlots.has(timeISO)) {
            hasBookingsForDay = true; // There are bookings for this day
        }
        currentTime.setMinutes(currentTime.getMinutes() + 30);
    }

    // If no bookings for the day, display full day availability
    if (!hasBookingsForDay) {
        availableTimes.push(`${currentTime.toDateString()} 00:00 - 23:30`);
    } else {
        // Reset currentTime for available slot calculation
        currentTime.setDate(today.getDate() + day);
        currentTime.setHours(startHour, 0, 0, 0);
        while (currentTime.getHours() < endHour) {
            const timeISO = currentTime.toISOString();
            if (!bookedSlots.has(timeISO)) {
                const formattedTime = `${String(currentTime.getHours()).padStart(2, '0')}:${String(currentTime.getMinutes()).padStart(2, '0')}`;
                availableTimes.push(`${currentTime.toDateString()} ${formattedTime}`);
            }
            currentTime.setMinutes(currentTime.getMinutes() + 30);
        }
    }
}

    // Display available times
    availableTimes.forEach(time => {
        const timeText = document.createElement('p');
        timeText.textContent = time;
        plcAvailableText.appendChild(timeText);
    });
}


async function initPLCBookingDisplay(plcId) {
    const bookings = await fetchPLCBookings(plcId);
    displayBookings(bookings);
}


document.addEventListener("DOMContentLoaded", initPLCBookingDisplay);



toggleDateTimeControls();
document.addEventListener("DOMContentLoaded", fetchBookedPLCs);
bookingDateSelectionCalender.addEventListener("change", fetchBookedPLCs);
timeRange.addEventListener('mouseup', fetchBookedPLCs);
timeRange.addEventListener('touchend', fetchBookedPLCs);
dateTimeNowCheckbox.addEventListener("change", toggleDateTimeControls, fetchBookedPLCs);

function setupRadioButtons() {
    const plcRadioButtons = document.querySelectorAll('input[name="plcRadio"]');

    plcRadioButtons.forEach(radioButton => {
        radioButton.addEventListener('change', (event) => {
            const selectedPlcId = event.target.id;
            initPLCBookingDisplay(selectedPlcId);
        });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    setupRadioButtons();

    const selectedRadioButton = document.querySelector('input[name="plcRadio"]:checked');
    if (selectedRadioButton) {
        initPLCBookingDisplay(selectedRadioButton.value);
    }
});


