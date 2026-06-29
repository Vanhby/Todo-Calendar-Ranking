//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using StreakHub.API.DataToObject;
//using StreakHub.API.Service;

//namespace StreakHub.API.Tests
//{
//    [TestFixture]
//    public class AuthTest_2
//    {
//        private Auth_Service _authService;
//        private static readonly string CustomDir = @"D:\Project_TT22_TestData";
//        private static readonly string InputFilePath = Path.Combine(CustomDir, "input.txt");
//        private static readonly string OutputFilePath = Path.Combine(CustomDir, "output.txt");

//        // Dùng Static List để tích lũy log từ các Test Case chạy riêng lẻ
//        private static List<string> _testResultsAccumulator;

//        [OneTimeSetUp]
//        public void OneTimeSetUp()
//        {
//            _authService = new Auth_Service(null!);
//            _testResultsAccumulator = new List<string>();

//            // Khởi tạo file log tổng hợp
//            _testResultsAccumulator.Add($"=== KẾT QUẢ KIỂM THỬ TỰ ĐỘNG - KỶ NIỆM {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
//            _testResultsAccumulator.Add("-------------------------------------------------------------------------");
//        }

//        /// <summary>
//        /// Hàm Đọc File dữ liệu đầu vào và chuyển đổi thành các TestCase cho NUnit nhận diện
//        /// </summary>
//        public static IEnumerable<TestCaseData> GetTestDataFromFile()
//        {
//            // Tự động tạo thư mục mẫu nếu chưa tồn tại
//            if (!Directory.Exists(CustomDir)) Directory.CreateDirectory(CustomDir);

//            if (!File.Exists(InputFilePath))
//            {
//                File.WriteAllLines(InputFilePath, new[] {
//                    "bangnd,password123",
//                    ",123456",
//                    "ab,123456",
//                    "bangnd@2026,password123"
//                });
//            }

//            string[] lines = File.ReadAllLines(InputFilePath);
//            int index = 1;

//            foreach (var line in lines)
//            {
//                if (string.IsNullOrWhiteSpace(line)) continue;

//                var parts = line.Split(',');
//                var username = parts.Length > 0 ? parts[0].Trim() : string.Empty;
//                var password = parts.Length > 1 ? parts[1].Trim() : string.Empty;

//                // Trả về dữ liệu dạng TestCaseData kèm theo tên hiển thị (SetArgDisplayNames) trên giao diện phần mềm
//                yield return new TestCaseData(username, password)
//                    .SetName($"TC_{index:D2}_User='{username}'_Pass='{password}'");

//                index++;
//            }
//        }

//        /// <summary>
//        /// HÀM TEST CHÍNH: NUnit sẽ tự động chạy hàm này LẶP LẠI nhiều lần ứng với số dòng trong file txt
//        /// </summary>
//        [Test]
//        [TestCaseSource(nameof(GetTestDataFromFile))]
//        public void ExecuteSingleTestCase(string username, string password)
//        {
//            bool isUserValid = _authService.CheckTaiKhoan(username, out string userError);
//            bool isPassValid = _authService.CheckMatKhau(password, out string passError);

//            // Gom log chi tiết cho từng case
//            string logDetail = $"\nInput [User: '{username}', Pass: '{password}']\n" +
//                               $"  -> CheckTaiKhoan: {(isUserValid ? "PASSED" : "FAILED")} | Msg: {userError}\n" +
//                               $"  -> CheckMatKhau:  {(isPassValid ? "PASSED" : "FAILED")} | Msg: {passError}";

//            _testResultsAccumulator.Add(logDetail);


//            if (!isUserValid)
//            {
//                Assert.Fail($"Thất bại tại hàm CheckTaiKhoan: {userError}");
//            }
//            if (!isPassValid)
//            {
//                Assert.Fail($"Thất bại tại hàm CheckMatKhau: {passError}");
//            }
//        }

//        [OneTimeTearDown]
//        public void OneTimeTearDown()
//        {
//            _testResultsAccumulator.Add("\n-------------------------------------------------------------------------");
//            _testResultsAccumulator.Add("=== KẾT THÚC QUÁ TRÌNH KIỂM THỬ ===");

//            // Ghi file output tổng hợp ra ổ D như bình thường
//            File.WriteAllLines(OutputFilePath, _testResultsAccumulator);
//            TestContext.Progress.WriteLine($"[INFO] Đã ghi log tổng hợp ra: {OutputFilePath}");
//        }
//    }
//}