"use strict";

const displayDate = document.getElementById("dateValue");
const displayTime = document.getElementById("timeValue");

function updateDateTime() {
    const currentDate = new Date();
    const currentDateString = currentDate.toLocaleDateString('de-DE');

    const hours = String(currentDate.getHours()).padStart(2, '0');
    const minutes = String(currentDate.getMinutes()).padStart(2, '0');

    displayDate.textContent = currentDateString;
    displayTime.textContent = `${hours}:${minutes}`;
}

updateDateTime();


async function fetchBookedPLCs() {
    const currentDate = new Date();
    const currentDateString = currentDate.toLocaleDateString('en-CA');

    const hours = String(currentDate.getHours()).padStart(2, '0');
    const minutes = String(currentDate.getMinutes()).padStart(2, '0'); 


    const startTimeString = `${hours}:${minutes}:00`; 
    const endTimeDate = new Date(currentDate); 
    endTimeDate.setHours(currentDate.getHours() + 1);
    const endHours = String(endTimeDate.getHours()).padStart(2, '0');
    const endMinutes = String(endTimeDate.getMinutes()).padStart(2, '0');
    const endTimeString = `${endHours}:${endMinutes}:00`; 

    const finalDateTimeStart = `${currentDateString}T${startTimeString}`;
    const finalDateTimeEnd = `${currentDateString}T${endTimeString}`;

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
            label.style.border = '12px solid #37A7BD';
        });
    } else {
        PLCentities.forEach(label => {
            const plcId = parseInt(label.getAttribute('id'));
            if (bookedPLCs.includes(plcId)) {
                label.style.border = '12px solid #BD4D37';
            } else {
                label.style.border = '12px solid #37A7BD';
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
    fetchBookedPLCs();
});


function setupRadioButtons() {
    const plcRadioButtons = document.querySelectorAll('input[name="plcRadio"]');

    plcRadioButtons.forEach(radioButton => {
        radioButton.addEventListener('change', (event) => {
            const selectedPlcId = event.target.id;
            initPLCBookingDisplay(selectedPlcId);
        });
    });
}

