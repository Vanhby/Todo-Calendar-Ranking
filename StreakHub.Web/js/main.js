// file: js/main.js
import { renderCalendar, setupCalendarEvents } from './components/calendar.js';
// import { setupTodo } from './components/todo.js'; // Sẽ tạo sau

// Khởi chạy các chức năng khi trang web load xong
document.addEventListener("DOMContentLoaded", () => {
    setupCalendarEvents();
    renderCalendar();
    // setupTodo();
});