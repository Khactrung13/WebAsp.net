using System;
using System.Globalization;

namespace SV20T1020607.Wed
{
		public static class Converter
		{
        /// <summary>
        /// Chuyển chuỗi s sang giá trị kiểu DateTime theo format được quy định
        /// hàm trả về null nếu không thành công
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);

            }
            catch
            {
                return null;
            }
        }
    }

    
}

