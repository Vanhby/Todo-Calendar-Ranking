

namespace StreakHub.API.ShareTest
{
    public class ShareCodeCheck
    {
        public List<ShareCodeModel> ValidateShareCode(List<ShareCodeModel> shareCodeList)
        {
            foreach (var item in shareCodeList)
            {
                if(item.ShareCode == "")
                {
                    item.RealResult = "không được rỗng";
                }
                else if (item.ShareCode.Length < 4 || item.ShareCode.Length > 16)
                {
                    item.RealResult = "Độ dài không hợp lệ";
                }
                else if (!item.ShareCode.All(char.IsLetterOrDigit))
                {
                    item.RealResult = "Chứa ký tự đặc biệt";
                }
                else
                {
                    item.RealResult = "hợp lệ";
                }
            }
            return shareCodeList;
        }
    }
}
