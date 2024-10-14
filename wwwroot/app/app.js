const APIPath = '../../'


const dateTimeNowCheckbox = document.getElementById("dateTimeNow");
const bookingDateSelectionCalender = document.getElementById("bookingDate");
const timeRange = document.getElementById("timeRange");

function updateTime() {
    const slider = document.getElementById("timeRange");
    const selectedTimeStartDisplay = document.getElementById("selectedTimeStart");
    const selectedTimeEndDisplay = document.getElementById("selectedTimeEnd");

    const startHour = Math.floor(slider.value / 2); 
    const startMinutes = (slider.value % 2) * 30; 

    const endValue = Math.min(Number(slider.value) + 1, 48); 
    const endHour = Math.floor(endValue / 2);
    const endMinutes = (endValue % 2) * 30; 

    const formattedStartTime = `${String(startHour).padStart(2, '0')}:${String(startMinutes).padStart(2, '0')}`;

    const formattedEndTime = `${String(endHour).padStart(2, '0')}:${String(endMinutes).padStart(2, '0')}`;
    selectedTimeStartDisplay.textContent = formattedStartTime;
    selectedTimeEndDisplay.textContent = formattedEndTime;
    fetchBookedPLCs();
}


function toggleDateTimeControls() {
    bookingDateSelectionCalender.disabled = dateTimeNowCheckbox.checked;
    timeRange.disabled = dateTimeNowCheckbox.checked;
}

dateTimeNowCheckbox.addEventListener("change", toggleDateTimeControls);

toggleDateTimeControls();


async function fetchBookedPLCs() {
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

    let adjustedStartTime = startTime === "24:00" ? "23:59" : startTime;
    let adjustedEndTime = endTime === "24:00" ? "23:59" : endTime;

    const finalDateTimeStart = `${selectedDate}T${adjustedStartTime}:00`;
    const finalDateTimeEnd = `${selectedDate}T${adjustedEndTime}:00`;

    console.log("Fetching booked PLCs for:", finalDateTimeStart, finalDateTimeEnd);

    try {
        const response = await fetch(`/api/requests/booked_PLCs?dateTimeStart=${encodeURIComponent(finalDateTimeStart)}&dateTimeEnd=${encodeURIComponent(finalDateTimeEnd)}`);

        if (!response.ok) {
            throw new Error('Failed to fetch booked PLCs');
        }

        const bookedPLCs = await response.json();
        console.log("Booked PLCs:", bookedPLCs);

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


document.addEventListener("DOMContentLoaded", fetchBookedPLCs);
