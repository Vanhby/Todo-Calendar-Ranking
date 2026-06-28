using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;
using StreakHub.API.Data;
using StreakHub.API.DataToObject;
using StreakHub.API.Models;
namespace StreakHub.API.Service
{
    public class Auth_Service
    {
        private readonly AppDbContext _context;

        
        
        public Auth_Service(AppDbContext context)
        {
            _context = context;
        }

        public bool CheckTaiKhoan(string username, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username không được để trống.";
                return false;
            }
            if (username.Length < 3 || username.Length > 20)
            {
                errorMessage = "Username phải từ 3 đến 20 ký tự.";
                return false;
            }
            // Chỉ cho phép chữ cái, chữ số, không chứa ký tự đặc biệt
            if (!Regex.IsMatch(username, "^[a-zA-Z0-9]+$"))
            {
                errorMessage = "Username không được chứa ký tự đặc biệt hoặc khoảng trắng.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool CheckMatKhau(string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Mật khẩu không được để trống.";
                return false;
            }
            if (password.Length < 6)
            {
                errorMessage = "Mật khẩu phải có độ dài tối thiểu từ 6 ký tự.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        


        public bool CheckEmail(string email, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Email không được để trống.";
                return false;
            }

            // Regex chuẩn kiểm tra định dạng email phổ biến
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex))
            {
                errorMessage = "Định dạng Email không hợp lệ.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            // BCrypt tự động sinh Salt ngẫu nhiên và gộp thẳng vào chuỗi kết quả trả về
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword)) return false;

            try
            {
                // BCrypt tự bóc tách Salt từ chuỗi hashed và thực hiện so khớp logic
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false; // Tránh crash ứng dụng nếu chuỗi hash truyền vào sai định dạng cấu trúc BCrypt
            }
        }


        public async Task<(bool IsSuccess, string Message)> Register(RegisterDto register)
        {
            // 1. Các bước CheckTaiKhoan, CheckEmail, CheckMatKhau cú pháp (Giữ nguyên)...
            if (!CheckTaiKhoan(register.Username, out string userError)) return (false, userError);
            if (!CheckEmail(register.Email, out string emailError)) return (false, emailError);
            if (!CheckMatKhau(register.Password, out string passError)) return (false, passError);

            // 2. Check trùng lặp database (Giữ nguyên)
            bool isUserExist = await _context.Users.AnyAsync(u => u.Username == register.Username || u.Email == register.Email);
            if (isUserExist) return (false, "Tài khoản hoặc Email đã tồn tại trong hệ thống.");

            try
            {
                // 3. Tiến hành BĂM MẬT KHẨU thô trước khi nạp vào Model
                string securePasswordHash = HashPassword(register.Password);

                var newUser = new User
                {
                    Username = register.Username,
                    PasswordHash = securePasswordHash, // Lưu chuỗi đã mã hóa an toàn xuống Neon DB
                    Email = register.Email,
                    AvatarUrl = "default_avatar.png",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return (true, "Đăng ký tài khoản thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi hệ thống khi lưu database: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<(bool IsSuccess, string Message)> Login(LoginDto login)
        {
            if (!CheckTaiKhoan(login.Username, out string usernameError)) return (false, usernameError);
            if (!CheckMatKhau(login.Password, out string passwordError)) return (false, passwordError);

            // Tìm User theo Username trước (Vì không thể so khớp mật khẩu thô trực tiếp bằng lệnh SQL được nữa)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null)
            {
                return (false, "Tài khoản hoặc mật khẩu không chính xác.");
            }

            // Tiến hành giải mã và xác thực mật khẩu thô với chuỗi Hash lấy từ DB lên
            bool isPasswordMatch = VerifyPassword(login.Password, user.PasswordHash);
            if (!isPasswordMatch)
            {
                return (false, "Tài khoản hoặc mật khẩu không chính xác.");
            }

            return (true, "Đăng nhập thành công!");
        }



    }
}
