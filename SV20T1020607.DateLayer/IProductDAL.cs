using System;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer
{
	public interface IProductDAL
	{
		/// <summary>
		/// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
		/// </summary>
		/// <param name="page"></param> Trang cần hiển thị 
		/// <param name="pageSize"></param> Số dòng trên mỗi trang (0 nếu không phân trang )
		/// <param name="searchValue"></param> Tên mặt hàng cần tìm ( Chuỗi rỗng nếu không tìm kiếm)
		/// <param name="CategoryID"></param> Mã loại hàng cần tìm ( 0 nếu không cần tìm )
		/// <param name="SupplierID"></param> Mã nhà cung cấp cần tìm ( 0 nếu không cần tìm)
		/// <param name="minPrice"></param> Mức giá nhỏ nhất trong khoảng giá cần tìm
		/// <param name="maxPrice"></param> Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không cần hạn chế mức giá lớn nhất)
		/// <returns></returns>
		IList<Product> List(int page = 1, int pageSize = 0, string searchValue = " ", int CategoryID = 0
			, int SupplierID = 0, decimal minPrice = 0 , decimal maxPrice=0);
        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue"></param> Tên mặt hàng cần tìm ( Chuỗi rỗng nếu không tìm kiếm)
        /// <param name="CategoryID"></param> Mã loại hàng cần tìm ( 0 nếu không cần tìm )
        /// <param name="SupplierID"></param> Mã nhà cung cấp cần tìm ( 0 nếu không cần tìm)
        /// <param name="minPrice"></param> Mức giá nhỏ nhất trong khoảng giá cần tìm
        /// <param name="maxPrice"></param> Mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không cần hạn chế mức giá lớn nhất)
        /// <returns></returns>
        int Count(string searchValue = "", int CategoryID = 0 , int SupplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
		/// <summary>
		/// Lấy thông tin mặt hàng theo mã hàng 
		/// </summary>
		/// <param name="ProductID"></param>
		/// <returns></returns>
		Product? Get(int ProductID);
		/// <summary>
		/// Bổ sung mặt hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		int Add(Product data);
		/// <summary>
		/// Cập nhật thông tin loại hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Update(Product data);
		/// <summary>
		/// Xóa mặt hàng theo mã mặt hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool Delele(int ProductID);
		/// <summary>
		/// Kiếm tra xem mặt hàng có được sử dụng không
		/// </summary>
		/// <param name="ProductID"></param>
		/// <returns></returns>
		bool IsUsed(int ProductID);
		/// <summary>
		/// Lấy danh sách ảnh của mặt hàng theo mã
		/// </summary>
		/// <param name="ProductID"></param>
		/// <returns></returns>
		IList<ProductPhoto> ListPhotos(int ProductID);
		/// <summary>
		/// Lấy thông tin 1 ảnh dựa theo mã
		/// </summary>
		/// <param name="PhotoID"></param>
		/// <returns></returns>
		ProductPhoto? GetPhoto(long PhotoID);
		/// <summary>
		/// Bổ sung 1 ảnh cho mặt hàng (hàm trả về mã của ảnh được bổ sung)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		long AddPhoto(ProductPhoto data);
		/// <summary>
		/// Cập nhật ảnh của mặt hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool UpdatePhoto(ProductPhoto data);
		/// <summary>
		/// Xóa ảnh của mặt hàng theo mã
		/// </summary>
		/// <param name="PhotoID"></param>
		/// <returns></returns>
		bool DeletePhoTo(long PhotoID);
		/// <summary>
		/// Lấy danh sách thuộc tính của mặt hàng theo mã mặt hàng
		/// </summary>
		/// <param name="ProductID"></param>
		/// <returns></returns>
		IList<ProductAttribute> ListAttributes(int ProductID);
		/// <summary>
		/// Lấy thông tin của thuộc tính theo mã thuộc tính
		/// </summary>
		/// <param name="AttributeID"></param>
		/// <returns></returns>
		ProductAttribute? GetAttribute(long AttributeID);
		/// <summary>
		/// Thêm thuộc tính cho mặt hàng 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		long AddAttribute(ProductAttribute data);
		/// <summary>
		/// Cập nhật thông tin thuộc tính của mặt hàng
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		bool UpdateAttribute(ProductAttribute data);
		/// <summary>
		/// Xóa thuộc tính theo mã thuộc tính
		/// </summary>
		/// <param name="AttributeID"></param>
		/// <returns></returns>
		bool DeleteAttrbute(long AttributeID);
        
    }
}

