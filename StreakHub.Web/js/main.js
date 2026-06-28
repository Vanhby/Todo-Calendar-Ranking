// file: js/main.js

import { setupCalendarEvents, renderCalendar } from './components/calendar.js';

document.addEventListener("DOMContentLoaded", () => {
    // 1. Khởi chạy Lịch
    setupCalendarEvents();
    renderCalendar();

    // 2. Khởi chạy TodoList (Tạm thời viết ở đây, sau này có thể tách ra todo.js)
    setupTodoMock();
});

function setupTodoMock() {
    const todayTasks = [
        { text: "Viết 30 phút nhật ký", done: true },
        { text: "Tập thể dục buổi sáng", done: true },
        { text: "Học 20 từ tiếng Anh mới", done: false },
        { text: "Hoàn thành 1 commit cho side project", done: false },
        { text: "Đọc 10 trang sách", done: false },
        { text: "Uống đủ 2 lít nước", done: true },
    ];

    const pendingList = document.getElementById("todo-pending");
    const doneList = document.getElementById("todo-done");
    const progress = document.getElementById("today-progress");
    const dateEl = document.getElementById("today-date");

    const now = new Date();
    dateEl.textContent = now.toLocaleDateString("vi-VN", { weekday: "long", day: "numeric", month: "numeric", year: "numeric" });

    let doneCount = 0;
    todayTasks.forEach((task) => {
        const li = document.createElement("li");
        if (task.done) {
            li.className = "flex items-center gap-2 p-2 rounded-md border border-[#30363d] opacity-40";
            li.innerHTML = `
                <svg height="16" viewBox="0 0 16 16" width="16" class="fill-[#39d353] flex-shrink-0" aria-hidden="true"><path d="M8 0a8 8 0 1 1 0 16A8 8 0 0 1 8 0Zm3.78 5.97a.751.751 0 0 0-1.06-.022L7 9.69 5.28 7.97a.751.751 0 0 0-1.042.018.751.751 0 0 0-.018 1.042l2.25 2.25a.75.75 0 0 0 1.06 0l4.25-4.25a.751.751 0 0 0 0-1.06Z"></path></svg>
                <span class="text-sm text-[#c9d1d9] line-through">${task.text}</span>`;
            doneList.appendChild(li);
            doneCount++;
        } else {
            li.className = "flex items-center gap-2 p-2 rounded-md border border-[#30363d] hover:border-[#c9d1d9]";
            li.innerHTML = `
                <svg height="16" viewBox="0 0 16 16" width="16" class="fill-[#6e7681] flex-shrink-0" aria-hidden="true"><path d="M8 0a8 8 0 1 1 0 16A8 8 0 0 1 8 0ZM1.5 8a6.5 6.5 0 1 0 13 0 6.5 6.5 0 0 0-13 0Z"></path></svg>
                <span class="text-sm text-[#c9d1d9]">${task.text}</span>`;
            pendingList.appendChild(li);
        }
    });

    progress.textContent = doneCount + "/" + todayTasks.length + " hoàn thành";
}