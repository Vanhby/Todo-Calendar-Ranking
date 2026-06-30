using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StreakHub.API.Data;
using StreakHub.API.Models;
using StreakHub.API.Services;
using System.Globalization;

namespace StreakHub.Tests
{
    [TestFixture]
    public class StreakServiceTests
    {
        private AppDbContext _context;
        private StreakService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new StreakService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private async Task LoadTodosFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines.Skip(1))
            {
                var data = line.Split(',');

                _context.Todos.Add(new Todo
                { 
                    UserId = int.Parse(data[0]),
                    TaskDate = DateOnly.ParseExact(
                        data[1],
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture),
                    IsCompleted = bool.Parse(data[2]),
                    Title = "Test Task",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        [Test]
        public async Task TC1_AllCompleted()
        {
            await LoadTodosFromCsv("TestData/TC1.csv");

            var result = await _service.GetUserStreakAsync(
                1,
                new DateOnly(2026, 6, 29));

            Assert.That(result.CurrentStreak, Is.EqualTo(2));
            Assert.That(result.TotalTasksCompleted, Is.EqualTo(2));
        }


        [Test]
        public async Task TC2_TodayNotCompleted()
        {
            await LoadTodosFromCsv("TestData/TC2.csv");

            var result = await _service.GetUserStreakAsync(
                1,
                new DateOnly(2026, 6, 29));

            Assert.That(result.CurrentStreak, Is.EqualTo(1));
            Assert.That(result.TotalTasksCompleted, Is.EqualTo(1));
        }


        [Test]
        public async Task TC3_PreviousDayNotCompleted()
        {
            await LoadTodosFromCsv("TestData/TC3.csv");

            var result = await _service.GetUserStreakAsync(
                1,
                new DateOnly(2026, 6, 29));

            Assert.That(result.CurrentStreak, Is.EqualTo(1));
            Assert.That(result.TotalTasksCompleted, Is.EqualTo(1));
        }
    }
}