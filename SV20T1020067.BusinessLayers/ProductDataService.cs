using System;
using System.Drawing.Printing;
using SV20T1020607.DataLayer;
using SV20T1020607.DataLayer.MySql;
using SV20T1020607.DomainModels;

namespace SV20T1020067.BusinessLayers
{
	public class ProductDataService
	{
        private static readonly IProductDAL productDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách măt hàng (Không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListOfProducts(string searchValue="")
        {
            return productDB.List(0,0,searchValue).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách măt hàng (dưới dạng phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="CategoryID"></param>
        /// <param name="SupplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public static List<Product> ListOfProducts(out int rowCount,int page = 1, int pageSize = 0, string searchValue = " ",
            int CategoryID = 0, int SupplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, CategoryID, SupplierID, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, CategoryID, SupplierID, minPrice, maxPrice).ToList();
        }
        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int ProductID)
        {
            return productDB.Get(ProductID);
        }
        /// <summary>
        /// Thêm mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Cập nhât thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// Xóa mặt hàng theo mã
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int ProductID)
        {
            return productDB.Delele(ProductID);
        }
        /// <summary>
        /// Kiểm tra xem mặt hàng có được sử dụng
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static bool IsUsedProduct(int ProductID)
        {
            return productDB.IsUsed(ProductID);
        }
        /// <summary>
        /// Lấy danh sách ảnh dựa theo mã mặt hàng
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListOfPhotos(int ProductID)
        {
            return (List<ProductPhoto>)productDB.ListPhotos(ProductID);
        }
        /// <summary>
        /// Lấy 1 ảnh theo id
        /// </summary>
        /// <param name="PhotoID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(long PhotoID)
        {
            return (ProductPhoto)productDB.GetPhoto(PhotoID);
        }
        /// <summary>
        /// Thêm ảnh của sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// Cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UppdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// Xóa ảnh của sản phẩm theo mã ảnh
        /// </summary>
        /// <param name="PhotoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long PhotoID)
        {
            return productDB.DeletePhoTo(PhotoID);
        }

        /// <summary>
        /// Lấy danh sách thuộc tính dựa theo mã mặt hàng
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListOfAttributes(int ProductID)
        {
            return (List<ProductAttribute>)productDB.ListAttributes(ProductID);
        }
        /// <summary>
        /// Lấy 1 thuộc tính theo id
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(long AtributeID)
        {
            return (ProductAttribute)productDB.GetAttribute(AtributeID);
        }
        /// <summary>
        /// Thêm thuộc tính của sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UppdateAttribue(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// Xóa thuộc tính của sản phẩm theo mã thuộc tính
        /// </summary>
        /// <param name="PhotoID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long AttributeID)
        {
            return productDB.DeleteAttrbute(AttributeID);
        }


    }
}

