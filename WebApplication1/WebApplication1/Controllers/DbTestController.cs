using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreakHub.API.Models_Generated;


namespace StreakHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Tiêm (Inject) AppDbContext vào để sử dụng
        public DbTestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-data")]
        public async Task<IActionResult> CheckData()
        {
            try
            {
                // 1. Đếm số lượng bản ghi trong các bảng (Ví dụ bảng Users và Todos)
                int userCount = await _context.Users.CountAsync();
                int todoCount = await _context.Todos.CountAsync();

                // 2. Lấy thử 3 User mới nhất (nếu có) để xem trực quan
                var sampleUsers = await _context.Users
                    .OrderByDescending(u => u.UserId)
                    .Take(3)
                    .Select(u => new { u.UserId, u.Username, u.Email, u.CreatedAt })
                    .ToListAsync();

                return Ok(new
                {
                    HasData = userCount > 0 || todoCount > 0,
                    TotalUsers = userCount,
                    TotalTodos = todoCount,
                    SampleUsers = sampleUsers,
                    Message = userCount > 0 ? "Database đã có dữ liệu!" : "Database trống (chưa có dữ liệu)."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("get-schema")]
        public async Task<IActionResult> GetSchema()
        {
            var tableSchema = new List<object>();

            // Mở kết nối tới DB dựa trên Connection String đã cấu hình
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                // Câu lệnh SQL đặc trưng của PostgreSQL để lấy tên bảng và tên cột
                command.CommandText = @"
            SELECT table_name, column_name, data_type 
            FROM information_schema.columns 
            WHERE table_schema = 'public'
            ORDER BY table_name, ordinal_position;";

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tableSchema.Add(new
                        {
                            Table = reader.GetString(0),
                            Column = reader.GetString(1),
                            DataType = reader.GetString(2)
                        });
                    }
                }
            }

            // Trả về chuỗi JSON chứa toàn bộ cấu trúc bảng cho bạn xem trên trình duyệt
            return Ok(tableSchema);
        }

        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                // Sử dụng lệnh CanConnectAsync để kiểm tra đường truyền tới server Neon
                bool canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Kết nối tới PostgreSQL (Neon) thành công!",
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = "Fail",
                        Message = "Không thể kết nối tới Database. Hãy kiểm tra lại cấu hình mạng hoặc chuỗi kết nối."
                    });
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết nếu quá trình kết nối bị crash
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
}