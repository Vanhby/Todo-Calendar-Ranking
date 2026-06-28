using StreakHub.API.DataToObject;
using StreakHub.API.Models_Generated;
using System.Text.RegularExpressions;

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

        public bool Login(LoginDto login, out string statusMessage)
        {
            // 1. Kiểm tra cú pháp của Tài khoản bằng hàm con
            if (!CheckTaiKhoan(login.Username, out string usernameError))
            {
                statusMessage = usernameError;
                return false;
            }

            // 2. Kiểm tra cú pháp của Mật khẩu bằng hàm con
            if (!CheckMatKhau(login.Password, out string passwordError))
            {
                statusMessage = passwordError;
                return false;
            }

            
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == login.Username && u.PasswordHash == login.Password);
            

            if (user == null)
            {
                statusMessage = "Tài khoản hoặc mật khẩu không chính xác dưới Database.";
                return false;
            }

            statusMessage = "Đăng nhập thành công!";
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
    }
}
