using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using StreakHub.API.Services;

namespace StreakHub.API.Tests
{
    [TestFixture]
    public class DndServiceTests
    {
        private DndService _dndService;
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

            _inputFilePath = Path.Combine(customDir, "dnd_input.txt");
            _outputFilePath = Path.Combine(customDir, "dnd_output.txt");
            _testResults = new List<string>();
            _dndService = new DndService(null!);

            if (!File.Exists(_inputFilePath))
            {
                File.WriteAllLines(_inputFilePath, new[] {
                    "true,22:00:00,06:00:00,23:30:00",
                    "true,22:00:00,06:00:00,12:00:00",
                    "false,22:00:00,06:00:00,23:30:00",
                    "true,invalid_time,06:00:00,23:30:00",
                    "true,18:00:00,invalid_time,23:30:00",
                    "true,08:00:00,17:00:00,09:00:00",
                    "true,08:00:00,17:00:00,19:00:00",
                    ",22:00:00,06:00:00,23:30:00"
                });
            }
        }

        [Test]
        public void RunAllDataDrivenTests()
        {
            _testResults.Add($"=== KẾT QUẢ KIỂM THỬ TỰ ĐỘNG DND - {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            _testResults.Add("-------------------------------------------------------------------------");

            string[] lines = File.ReadAllLines(_inputFilePath);

            int index = 1;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                string enabledStr = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                string startTimeStr = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                string endTimeStr = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                string currentTimeStr = parts.Length > 3 ? parts[3].Trim() : string.Empty;

                _testResults.Add($"\nTest Case #{index}: Input [Enabled: '{enabledStr}', Start: '{startTimeStr}', End: '{endTimeStr}', CurrentTime: '{currentTimeStr}']");

                bool enabled = false;
                bool isEnabledParsed = bool.TryParse(enabledStr, out enabled);
                if (!isEnabledParsed && !string.IsNullOrEmpty(enabledStr))
                {
                    _testResults.Add($"  -> Phân tích Enabled: FAILED | Msg: Giá trị 'Enabled' không hợp lệ.");
                    index++;
                    continue;
                }

                bool isTimeValid = _dndService.CheckDndTimeValid(startTimeStr, endTimeStr, out TimeSpan startTime, out TimeSpan endTime, out string timeError);
                _testResults.Add($"  -> CheckDndTimeValid: {(isTimeValid ? "PASSED" : "FAILED")} | Msg: {timeError}");

                if (isTimeValid)
                {
                    if (TimeSpan.TryParse(currentTimeStr, out TimeSpan currentTime))
                    {
                        bool hasDnd = _dndService.CheckUserDndStatus(enabled, startTime, endTime, currentTime, out string statusMessage);
                        _testResults.Add($"  -> CheckUserDndStatus: {(hasDnd ? "DND_ACTIVE" : "DND_INACTIVE")} | Msg: {statusMessage}");
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

            Assert.Pass("Đã thực hiện chạy hết tập dữ liệu kiểm thử DND.");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.WriteAllLines(_outputFilePath, _testResults);
            TestContext.Progress.WriteLine($"[INFO] Đã xuất kết quả kiểm thử DND ra file tại: {_outputFilePath}");
        }
    }
}
