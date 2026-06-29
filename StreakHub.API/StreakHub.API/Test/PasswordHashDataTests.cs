using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using StreakHub.API.Service;

namespace StreakHub.API.Tests
{
    [TestFixture]
    public class PasswordHashDataTests
    {
        private Auth_Service _authService;
        private static readonly string CustomDir = @"D:\Project_TT22_TestData";
        private static readonly string InputFilePath = Path.Combine(CustomDir, "password_input.txt");
        private static readonly string OutputFilePath = Path.Combine(CustomDir, "password_output.txt");

        // Sử dụng Concurrent làm bộ lưu trữ dùng chung cho tất cả các luồng chạy test
        private static List<string> _testResultsAccumulator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _authService = new Auth_Service(null!);
            _testResultsAccumulator = new List<string>();

            _testResultsAccumulator.Add($"=== KẾT QUẢ KIỂM THỬ HÀM BĂM MẬT KHẨU {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            _testResultsAccumulator.Add("-----------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// Đọc file dữ liệu thô và nạp kịch bản động dựa vào logic phân tích trong code
        /// </summary>
        public static IEnumerable<TestCaseData> GetPasswordTestData()
        {
            if (!Directory.Exists(CustomDir)) Directory.CreateDirectory(CustomDir);

            // Tự động sinh file input dạng dữ liệu thô nếu chưa tồn tại
            if (!File.Exists(InputFilePath))
            {
                File.WriteAllLines(InputFilePath, new[] {
                    "123456,123456",
                    "StreakHub2026,WrongPass2026",
                    ","
                });
            }

            string[] lines = File.ReadAllLines(InputFilePath);
            int index = 1;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                var rawPassword = parts.Length > 0 ? parts[0] : string.Empty;
                var verifyPassword = parts.Length > 1 ? parts[1] : string.Empty;

                // TỰ ĐỘNG PHÂN TÍCH KỊCH BẢN TRONG CODE KHÔNG CẦN FILE TXT MÔ TẢ
                string autoDescription;
                if (string.IsNullOrEmpty(rawPassword) && string.IsNullOrEmpty(verifyPassword))
                    autoDescription = "Biên trống cả hai trường";
                else if (string.IsNullOrEmpty(rawPassword))
                    autoDescription = "Biên trống mật khẩu gốc";
                else if (string.IsNullOrEmpty(verifyPassword))
                    autoDescription = "Biên trống mật khẩu so khớp";
                else if (rawPassword == verifyPassword)
                    autoDescription = "Mật khẩu trùng khớp hoàn toàn";
                else
                    autoDescription = "Mật khẩu sai lệch dữ liệu";

                yield return new TestCaseData(rawPassword, verifyPassword, autoDescription)
                    .SetName($"Hash_TC_{index:D2}_{autoDescription.Replace(" ", "_")}");
                index++;
            }
        }

        [Test]
        [TestCaseSource(nameof(GetPasswordTestData))]
        public void ExecutePasswordLogicTest(string rawPassword, string verifyPassword, string description)
        {
            // Lấy tên Test Case hiện tại đang hiển thị trên giao diện làm tiêu đề log
            string currentTestName = TestContext.CurrentContext.Test.Name;

            var localLog = new List<string>
            {
                $"\n[Mã Test Case]: {currentTestName}",
                $"  -> Dữ liệu thô đầu vào: Raw='{rawPassword}', Verify='{verifyPassword}'"
            };

            // 1. Thực hiện hành động băm mật khẩu thô
            string generatedHash = _authService.HashPassword(rawPassword);
            localLog.Add($"  -> Chuỗi mã hóa sinh ra: '{generatedHash}'");

            // 2. Thực hiện hành động xác thực kiểm thử
            bool verificationResult = _authService.VerifyPassword(verifyPassword, generatedHash);
            localLog.Add($"  -> Kết quả đối khớp: {(verificationResult ? "MATCHED (Khớp)" : "NOT MATCHED (Không khớp)")}");

            // Khóa luồng tích lũy log tránh xung đột dữ liệu
            lock (_testResultsAccumulator)
            {
                _testResultsAccumulator.AddRange(localLog);
            }

            // 3. Đo lường Assert dựa trên bảng quyết định tự động trong code
            if (string.IsNullOrEmpty(rawPassword) || string.IsNullOrEmpty(verifyPassword))
            {
                Assert.That(verificationResult, Is.False, "Lỗi: Hệ thống phải trả về False khi có trường trống.");
            }
            else if (rawPassword == verifyPassword)
            {
                Assert.That(verificationResult, Is.True, "Lỗi: Xác thực thất bại dù dữ liệu trùng khớp.");
            }
            else
            {
                Assert.That(verificationResult, Is.False, "Lỗi: Xác thực thành công dù mật khẩu bị sai.");
            }

            
            lock (_testResultsAccumulator)
            {
                File.WriteAllLines(OutputFilePath, _testResultsAccumulator);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            lock (_testResultsAccumulator)
            {
                _testResultsAccumulator.Add("\n-----------------------------------------------------------------------------------------");
                _testResultsAccumulator.Add("=== KẾT THÚC QUÁ TRÌNH KIỂM THỬ HÀM BĂM MẬT KHẨU ===");
                File.WriteAllLines(OutputFilePath, _testResultsAccumulator);
            }
        }
    }
}