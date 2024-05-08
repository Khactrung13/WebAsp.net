using System;
namespace SV20T1020067.BusinessLayers
{

    /// <summary>
    /// Khoi tao va luu tru cac thong tin cau hinh  cua BussinessLayer
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Chuỗi kết nối đến cơ sở dữ liệu
        /// </summary>
        public static string ConnectionString { get; private set; } = "";

        /// <summary>
        /// Hàm khởi tạo cho Business Layer(Hàm này được gọi trước khi chạy ứng dụng)
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}

