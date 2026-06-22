// file: js/components/calendar.js

import { contribColors, monthNames } from '../utils/dateUtils.js';
import { fetchCalendarData } from '../api/calendarApi.js';

const calendar = document.getElementById("calendar");
const title = document.getElementById("cal-title");
const monthSelect = document.getElementById("month-select");
const yearSelect = document.getElementById("year-select");

const today = new Date();
let viewYear = today.getFullYear();
let viewMonth = today.getMonth(); // 0-11
const currentUserId = 1; // Tạm fix cứng ID = 1

export function setupCalendarEvents() {
    // Đổ option cho bộ chọn tháng
    monthNames.forEach((name, idx) => {
        const opt = document.createElement("option");
        opt.value = idx;
        opt.textContent = name;
        opt.className = "bg-[#161b22]";
        monthSelect.appendChild(opt);
    });

    // Đổ option cho bộ chọn năm (10 năm trước -> 5 năm sau)
    for (let y = today.getFullYear() - 10; y <= today.getFullYear() + 5; y++) {
        const opt = document.createElement("option");
        opt.value = y;
        opt.textContent = y;
        opt.className = "bg-[#161b22]";
        yearSelect.appendChild(opt);
    }

    document.getElementById("prev-month").addEventListener("click", () => changeMonth(-1));
    document.getElementById("next-month").addEventListener("click", () => changeMonth(1));
    document.getElementById("today-btn").addEventListener("click", () => {
        viewYear = today.getFullYear();
        viewMonth = today.getMonth();
        renderCalendar();
    });
    
    monthSelect.addEventListener("change", (e) => { viewMonth = parseInt(e.target.value); renderCalendar(); });
    yearSelect.addEventListener("change", (e) => { viewYear = parseInt(e.target.value); renderCalendar(); });
}

function changeMonth(delta) {
    viewMonth += delta;
    if (viewMonth < 0) { viewMonth = 11; viewYear--; }
    else if (viewMonth > 11) { viewMonth = 0; viewYear++; }
    renderCalendar();
}

export async function renderCalendar() {
    calendar.innerHTML = ""; 
    title.textContent = monthNames[viewMonth] + " " + viewYear;
    monthSelect.value = viewMonth;
    yearSelect.value = viewYear;

    // Gọi API lấy dữ liệu thật
    const apiData = await fetchCalendarData(currentUserId, viewYear, viewMonth);

    const firstDay = new Date(viewYear, viewMonth, 1).getDay();
    const daysInMonth = new Date(viewYear, viewMonth + 1, 0).getDate();

    for (let i = 0; i < 42; i++) {
        const cell = document.createElement("div");
        const dayNum = i - firstDay + 1;
        const inMonth = dayNum >= 1 && dayNum <= daysInMonth;

        if (inMonth) {
            let level = 0; 
            if (apiData && apiData.days) {
                const backendMonthStr = String(viewMonth + 1).padStart(2, '0');
                const dayStr = String(dayNum).padStart(2, '0');
                const targetDateStr = `${viewYear}-${backendMonthStr}-${dayStr}`;

                const dayInfo = apiData.days.find(d => d.date === targetDateStr);
                if (dayInfo) {
                    level = dayInfo.colorLevel;
                }
            }

            const color = contribColors[level];
            const border = color === "#161b22" ? "#30363d" : "transparent";
            
            cell.className = "aspect-square rounded-md flex items-start justify-end p-1 text-xs font-medium border hover:border-white cursor-pointer";
            cell.style.backgroundColor = color;
            cell.style.borderColor = border;
            cell.style.color = color === "#161b22" ? "#6e7681" : "#ffffff";
            cell.textContent = dayNum;

            if (dayNum === today.getDate() && viewMonth === today.getMonth() && viewYear === today.getFullYear()) {
                cell.style.borderColor = "#58a6ff";
            }
        } else {
            cell.className = "aspect-square rounded-md border border-transparent opacity-20";
        }
        calendar.appendChild(cell);
    }
}