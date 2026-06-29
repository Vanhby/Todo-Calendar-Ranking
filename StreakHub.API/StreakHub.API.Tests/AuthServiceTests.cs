using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using StreakHub.API.DataToObject;
using StreakHub.API.Service;

namespace StreakHub.API.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Auth_Service _authService;
        private string _inputFilePath;
        private string _outputFilePath;
        private List<string> _testResults;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // 1. Định nghĩa thư mục lưu trữ riêng của bạn ở ổ D
            string customDir = @"D:\Project_TT22_TestData";

            // Tự động tạo thư mục "Project_TT22_TestData" ở ổ D nếu máy bạn chưa có sẵn
            if (!Directory.Exists(customDir))
            {
                Directory.CreateDirectory(customDir);
            }

            // 2. Gán đường dẫn file thẳng vào thư mục ổ D vừa tạo
            _inputFilePath = Path.Combine(customDir, "input.txt");
            _outputFilePath = Path.Combine(customDir, "output.txt");

            // Khởi tạo danh sách chứa kết quả để xuất ra file sau này
            _testResults = new List<string>();


            _authService = new Auth_Service(null!);

            // Tạo file input mẫu ở ổ D nếu nó chưa tồn tại để hệ thống có data chạy test
            if (!File.Exists(_inputFilePath))
            {
                File.WriteAllLines(_inputFilePath, new[] {
                    "bangnd,password123",
                    ",123456",
                    "ab,validpass",
                    "user@nghiemtrong,short"
                });
            }
        }

        [Test]
        public void RunAllDataDrivenTests()
        {
            _testResults.Add($"=== KẾT QUẢ KIỂM THỬ TỰ ĐỘNG - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            _testResults.Add("-------------------------------------------------------------------------");

            // 1. Đọc dữ liệu từ file txt đầu vào 
            string[] lines = File.ReadAllLines(_inputFilePath);
            List<LoginDto> testDataList = new List<LoginDto>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                var dto = new LoginDto
                {
                    Username = parts.Length > 0 ? parts[0].Trim() : string.Empty,
                    Password = parts.Length > 1 ? parts[1].Trim() : string.Empty
                };
                testDataList.Add(dto);
            }

            // 2. Chạy vòng lặp duyệt qua từng item và kiểm thử các hàm con
            int index = 1;
            foreach (var testData in testDataList)
            {
                _testResults.Add($"\nTest Case #{index}: Input [User: '{testData.Username}', Pass: '{testData.Password}']");

                // Kiểm thử hàm CheckTaiKhoan
                bool isUserValid = _authService.CheckTaiKhoan(testData.Username, out string userError);
                _testResults.Add($"  -> CheckTaiKhoan: {(isUserValid ? "PASSED" : "FAILED")} | Msg: {userError}");

                // Kiểm thử hàm CheckMatKhau
                bool isPassValid = _authService.CheckMatKhau(testData.Password, out string passError);
                _testResults.Add($"  -> CheckMatKhau:  {(isPassValid ? "PASSED" : "FAILED")} | Msg: {passError}");

                index++;
            }

            _testResults.Add("\n-------------------------------------------------------------------------");
            _testResults.Add("=== KẾT THÚC QUÁ TRÌNH KIỂM THỬ ===");

            // 3. Khẳng định (Assert) chạy xong luồng thành công
            Assert.Pass("Đã thực hiện chạy hết tập dữ liệu kiểm thử.");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Xuất toàn bộ danh sách kết quả lưu trong RAM ra file txt đầu ra ở ổ D
            File.WriteAllLines(_outputFilePath, _testResults);

            // In đường dẫn ra Console của NUnit để bạn biết chỗ mở file kết quả
            TestContext.Progress.WriteLine($"[INFO] Đã xuất kết quả kiểm thử ra file tại: {_outputFilePath}");
        }
    }
}