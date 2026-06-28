using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using StreakHub.API.Services;

namespace StreakHub.API.Tests
{
    [TestFixture]
    public class ReminderServiceTests
    {
        private ReminderService _reminderService;
        private string _inputFilePath;
        private string _outputFilePath;
        private List<string> _testResults;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            string customDir = OperatingSystem.IsWindows()
                ? @"D:\Project_TT22_TestData"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Project_TT22_TestData");

            if (!Directory.Exists(customDir))
            {
                Directory.CreateDirectory(customDir);
            }

            _inputFilePath = Path.Combine(customDir, "reminder_input.txt");
            _outputFilePath = Path.Combine(customDir, "reminder_output.txt");
            _testResults = new List<string>();

            _reminderService = new ReminderService(null!, null!);

            if (!File.Exists(_inputFilePath))
            {
                File.WriteAllLines(_inputFilePath, new[] {
                    "2026-06-28T20:30:00,2026-06-28T20:35:00,false",
                    "2026-06-28T20:30:00,2026-06-28T20:25:00,false",
                    "2026-06-28T20:30:00,2026-06-28T20:35:00,true",
                    "invalid_date,2026-06-28T20:35:00,false",
                    ",2026-06-28T20:35:00,false"
                });
            }
        }

        [Test]
        public void RunAllDataDrivenTests()
        {
            _testResults.Add($"=== KẾT QUẢ KIỂM THỬ TỰ ĐỘNG REMINDER - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            _testResults.Add("-------------------------------------------------------------------------");

            string[] lines = File.ReadAllLines(_inputFilePath);

            int index = 1;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                string notifyTimeStr = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                string currentTimeStr = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                string isDndActiveStr = parts.Length > 2 ? parts[2].Trim() : string.Empty;

                _testResults.Add($"\nTest Case #{index}: Input [NotifyTime: '{notifyTimeStr}', CurrentTime: '{currentTimeStr}', IsDndActive: '{isDndActiveStr}']");

                bool isDndActive = false;
                bool isDndParsed = bool.TryParse(isDndActiveStr, out isDndActive);
                if (!isDndParsed && !string.IsNullOrEmpty(isDndActiveStr))
                {
                    _testResults.Add($"  -> Phân tích IsDndActive: FAILED | Msg: Giá trị 'IsDndActive' không hợp lệ.");
                    index++;
                    continue;
                }

                bool isTimeValid = _reminderService.CheckReminderTimeValid(notifyTimeStr, out DateTime notifyTime, out string timeError);
                _testResults.Add($"  -> CheckReminderTimeValid: {(isTimeValid ? "PASSED" : "FAILED")} | Msg: {timeError}");

                if (isTimeValid)
                {
                    if (DateTime.TryParse(currentTimeStr, out DateTime currentTime))
                    {
                        bool isTriggered = _reminderService.CheckReminderStatus(notifyTime, currentTime, isDndActive, out string statusMessage);
                        _testResults.Add($"  -> CheckReminderStatus: {(isTriggered ? "TRIGGERED" : "BLOCKED_OR_FUTURE")} | Msg: {statusMessage}");
                    }
                    else
                    {
                        _testResults.Add($"  -> Phân tích CurrentTime: FAILED | Msg: Định dạng thời gian hiện tại không hợp lệ.");
                    }
                }

                index++;
            }

            _testResults.Add("\n-------------------------------------------------------------------------");
            _testResults.Add("=== KẾT THÚC QUÁ TRÌNH KIỂM THỬ ===");

            Assert.Pass("Đã thực hiện chạy hết tập dữ liệu kiểm thử Reminder.");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.WriteAllLines(_outputFilePath, _testResults);
            TestContext.Progress.WriteLine($"[INFO] Đã xuất kết quả kiểm thử Reminder ra file tại: {_outputFilePath}");
        }
    }
}
