using NUnit.Framework;
using StreakHub.API.DTOs;
using StreakHub.API.Services;

namespace StreakHub.API.Tests
{
    [TestFixture]
    public class ShareCodeCheckTests
    {
        private ShareService _ShareService;

        [SetUp]
        public void Setup()
        {
            _ShareService = new ShareService();
        }

        [TestCase("Ab1", false)]   //min-1
        [TestCase("Ab12", true)]  //min
        [TestCase("Abc12", true)] //min+1
        [TestCase("Abcde12345", true)]    //mid
        [TestCase("Code12345123451", true)]   //max-1
        [TestCase("Code123451234512", true)]  //max
        [TestCase("Code1234512345123", false)] //max+1

        //ký tự đặc biệt
        [TestCase("", false)]
        [TestCase("Abc$$", false)]
        [TestCase("A b c", false)]
        [TestCase("Code_123", false)]

        //dữ liệu lỗi giả
        [TestCase("Code_123", true)]
        [TestCase("", true)]

        [Test]
        public void ValidateShareCode_CheckShareCodeValidity_ReturnsExpectedBoolean(string inputShareCode, bool expectedResult)
        {
            bool actualResult = _ShareService.ValidateShareCode(inputShareCode);

            Assert.That(actualResult, Is.EqualTo(expectedResult), 
                $"Lỗi: Input '{inputShareCode}' mong đợi trả về {expectedResult} nhưng lại ra {actualResult}");
        }
    }
}