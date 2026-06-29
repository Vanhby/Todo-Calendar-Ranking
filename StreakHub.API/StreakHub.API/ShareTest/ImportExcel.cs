using OfficeOpenXml;

namespace StreakHub.API.ShareTest
{
    public class ImportExcel
    {
        public List<ShareCodeModel> ImportShareCode(string filePath)
        {
            var shareCodeList = new List<ShareCodeModel>();
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException("Không tìm thấy file Excel đầu vào: " + filePath);


            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Lấy sheet đầu tiên (Index có thể là 0 hoặc 1 tùy phiên bản EPPlus, thường collection là 0)
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                // Bắt đầu từ dòng 2 để bỏ qua dòng tiêu đề
                for (int row = 2; row <= rowCount; row++)
                {
                    var model = new ShareCodeModel();

                    // Cột 1: TC (Id)
                    if (int.TryParse(worksheet.Cells[row, 1].Text, out int id))
                    {
                        model.Id = id;
                    }

                    // Cột 4: Dữ liệu đầu vào
                    model.ShareCode = worksheet.Cells[row, 4].Text ?? string.Empty;

                    // Cột 5: KQMD
                    model.ExpectedResult = worksheet.Cells[row, 5].Text ?? string.Empty;

                    shareCodeList.Add(model);
                }
            }
            return shareCodeList;
        }
    }
}
