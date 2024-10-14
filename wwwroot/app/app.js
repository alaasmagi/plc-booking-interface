const APIPath = '../../'


const dateTimeNowCheckbox = document.getElementById("dateTimeNow");
const bookingDateSelectionCalender = document.getElementById("bookingDate");
const timeRange = document.getElementById("timeRange");

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

toggleDateTimeControls();
document.addEventListener("DOMContentLoaded", fetchBookedPLCs);
bookingDateSelectionCalender.addEventListener("change", fetchBookedPLCs);
timeRange.addEventListener('mouseup', fetchBookedPLCs);
timeRange.addEventListener('touchend', fetchBookedPLCs);
dateTimeNowCheckbox.addEventListener("change", toggleDateTimeControls, fetchBookedPLCs);

