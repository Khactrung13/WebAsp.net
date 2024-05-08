using System;
using SV20T1020607.DomainModels;

namespace SV20T1020607.Wed.Models
{
	/// <summary>
	/// Lớp cơ sở cho các lớp biểu diễn dữ liệu là kết quả của thao tác tìm kiếm, phân trang
	/// </summary>
	public abstract class BasePaginationResult
	{
		public int Page { get; set; }
        public int PageSize { get; set; }
		public string SearchValue { get; set; } = "";
        public int RountCount { get; set; }
        public int PageCount {
			get
			{
				if (PageSize == 0)
					return 1;
				int c = RountCount / PageSize;
				if (RountCount % PageSize > 0)
					c += 1;
				return c;
			}
		}
		public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }

    }
	/// <summary>
	/// Kết quả tìm kiếm khách hàng
	/// </summary>
	public class CustomerSearchResult : BasePaginationResult
	{
		public List<Customer> Data { get; set; } = new List<Customer>();
	}
	/// <summary>
	/// Kết quả tìm kiếm nhà cung cấp
	/// </summary>
	public class SupplierSearchResult : BasePaginationResult
	{
		public List<Supplier> Data { get; set; } = new List<Supplier>();
	}
	/// <summary>
	/// Kết quả tìm kiếm shipper
	/// </summary>
	public class ShipperSearchResult : BasePaginationResult
	{
		public List<Shipper> Data { get; set; } = new List<Shipper>();
	}
	/// <summary>
	/// Kết quả tìm kiếm Nhân viên
	/// </summary>
	public class EmployeeSearchResult : BasePaginationResult
	{
		public List<Employee> Data { get; set; } = new List<Employee>();
	}
    public class EmployeeAccountSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; } = new List<Employee>();
    }
    /// <summary>
    /// Kết quả tìm kiếm loại hàng
    /// </summary>
    public class CategorySearchResult : BasePaginationResult
	{
		public List<Category> Data { get; set; } = new List<Category>();
	}


	/// <summary>
	/// Kết quả tìm kiếm sản phẩm
	/// </summary>
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; } = new List<Product>();
    }


    public class OrderSearchResult : BasePaginationResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }
}

