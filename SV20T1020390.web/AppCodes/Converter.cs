using System.Globalization;

namespace SV20T1020390.web
{
    public static class Converter
    {
        /// <summary>
        /// Chuyển chuỗi S sang giá trị kiểu DateTime theo các format được qui định
        /// (Hàm trả về null nếu chuyển không thành công)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "dd/MM/yyyy;dd-MM-yyyy;dd.MM.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }catch
            {
                return null;
            }
        }
    }
}
