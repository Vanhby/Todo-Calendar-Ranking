// file: js/api/calendarApi.js

export async function fetchCalendarData(userId, year, month) {
    try {
        const backendMonth = month + 1; 
        // ĐÃ ĐỔI SANG CỔNG 5000 ĐỂ KHỚP VỚI BACKEND HIỆN TẠI
        const url = `http://localhost:5000/api/calendar/month/${userId}?year=${year}&month=${backendMonth}`;
        
        const response = await fetch(url);
        if (!response.ok) throw new Error("Lỗi mạng hoặc API chưa chạy!");
        
        return await response.json(); 
    } catch (error) {
        console.error("Không lấy được dữ liệu:", error);
        return null;
    }
}




public void TestGetTodo(int i){

    //fake data
        Id = 1
    //
        var l = gettodo(Id);
    //
        Assert.AreEqual(Todo.type, l.Count);
}