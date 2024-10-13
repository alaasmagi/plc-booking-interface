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
}