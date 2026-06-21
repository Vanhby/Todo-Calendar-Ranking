// file: js/api/calendarApi.js
export async function fetchCalendarData(userId, year, month) {
    try {
        const backendMonth = month + 1; 
        const url = `http://localhost:5000/api/calendar/month/${userId}?year=${year}&month=${backendMonth}`;
        
        const response = await fetch(url);
        if (!response.ok) throw new Error("Lỗi mạng hoặc API chưa chạy!");
        return await response.json();
    } catch (error) {
        console.error("Không lấy được dữ liệu:", error);
        return null;
    }
}