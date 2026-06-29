using NUnit.Framework;
using OfficeOpenXml;

namespace StreakHub.API.ShareTest
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void run()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Ca Nhan");

            List<ShareCodeModel> shareCodeList, validatedshareCodeList;
            ImportExcel importExcel = new ImportExcel();
            ShareCodeCheck shareCodeCheck = new ShareCodeCheck();
            ExportExcel exportExcel = new ExportExcel();

            shareCodeList = importExcel.ImportShareCode(@"E:\test\Input.xlsx");
            validatedshareCodeList = shareCodeCheck.ValidateShareCode(shareCodeList);
            exportExcel.ExportShareCode(validatedshareCodeList, @"E:\test\TestResult.xlsx");

            Assert.Pass("Đã thực thi test và xuất file Excel thành công.");
        }
    }
}
