"use strict";

const timeRange = document.getElementById("timeRange");
const selectedTimeStartDisplay = document.getElementById("selectedTimeStart");
const selectedTimeEndDisplay = document.getElementById("selectedTimeEnd");

function updateTime() {
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

async function fetchBookedPLCs() {
    const startTimeInput = document.getElementById("selectedTimeStart").textContent;
    const endTimeInput = document.getElementById("selectedTimeEnd").textContent;
    const currentDate = new Date();
    const currentDateString = currentDate.toLocaleDateString('en-CA');
    const finalDateTimeStart = `${currentDateString}T${startTimeInput}:00`;
    const finalDateTimeEnd = `${currentDateString}T${endTimeInput}:00`;

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
    const PLCentities = document.querySelectorAll('.plc-label');

    if (bookedPLCs.length === 0) {
        PLCentities.forEach(label => {
            label.style.border = '6px solid #37A7BD';
        });
    } else {
        PLCentities.forEach(label => {
            const plcId = parseInt(label.getAttribute('id')); 
            if (bookedPLCs.includes(plcId)) {
                label.style.border = '6px solid #BD4D37';
            } else {
                label.style.border = '6px solid #37A7BD';
            }
        });
    }

    deleteRequests;
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

document.addEventListener("DOMContentLoaded", () => {
    setupRadioButtons();
    initializeSlider();
    fetchBookedPLCs();
});

timeRange.addEventListener('mouseup', handleSliderChange);
timeRange.addEventListener('touchend', handleSliderChange);

function setupRadioButtons() {
    const plcRadioButtons = document.querySelectorAll('input[name="plcRadio"]');

    plcRadioButtons.forEach(radioButton => {
        radioButton.addEventListener('change', (event) => {
            const selectedPlcId = event.target.id;
            initPLCBookingDisplay(selectedPlcId);
        });
    });
}

function initializeSlider() {
    const currentDate = new Date();
    const currentHour = currentDate.getHours();
    const currentMinutes = currentDate.getMinutes();
    


    const formattedDate = new Intl.DateTimeFormat('en-GB', {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    }).format(currentDate);

    const sliderValue = (currentHour * 2) + Math.floor(currentMinutes / 30);

    timeRange.value = Math.min(Math.max(sliderValue, 0), 47);

    updateTime();
}

function handleSliderChange() {
    updateTime();
    fetchBookedPLCs();
}
