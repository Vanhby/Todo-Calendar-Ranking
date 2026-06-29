using OfficeOpenXml;

namespace StreakHub.API.ShareTest
{
    public class ExportExcel
    {
        public void ExportShareCode(List<ShareCodeModel> shareCodeList, string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
                fileInfo.Delete();

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("TestResults");

                // Tạo Header
                worksheet.Cells[1, 1].Value = "TC";
                worksheet.Cells[1, 2].Value = "Dữ liệu đầu vào";
                worksheet.Cells[1, 3].Value = "Kết quả mong đợi (KQMD)";
                worksheet.Cells[1, 4].Value = "Kết quả thực tế (RealResult)";

                int row = 2;
                foreach (var item in shareCodeList)
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.ShareCode;
                    worksheet.Cells[row, 3].Value = item.ExpectedResult;
                    worksheet.Cells[row, 4].Value = item.RealResult;
                    row++;
                }
                worksheet.Cells.AutoFitColumns();
                package.Save();
            }
        }
    }
}