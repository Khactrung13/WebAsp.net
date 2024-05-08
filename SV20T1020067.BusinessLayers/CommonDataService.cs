using System;
using SV20T1020607.DataLayer;
using SV20T1020607.DataLayer.MySql;
using SV20T1020607.DomainModels;

namespace SV20T1020067.BusinessLayers
{
	/// <summary>
	/// Cung cap cac chuc nang nghiep vu lien quan den du lieu "chung"
	/// </summary>
	public static class CommonDataService
	{
		private static readonly ICommonDAL<Province> provinceDB;
		private static readonly ICommonDAL<Customer> customerDB;
		private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<StatusOrder> statusDB;

        /// <summary>
        /// Contrustor
        /// </summary>
        static CommonDataService()
		{
			string connectionString = Configuration.ConnectionString;

			provinceDB = new ProvinceDAL(connectionString);

			customerDB = new CustomerDAL(connectionString);

			supplierDB = new SupplierDAL(connectionString);

            shipperDB = new ShipperDAL(connectionString);

            employeeDB = new EmployeeDAL(connectionString);

            categoryDB = new CategoryDAL(connectionString);
            statusDB = new StatusOrderDAL(connectionString);
        }
		/// <summary>
		/// Lay danh sach cac tinh thanh 
		/// </summary>
		/// <returns></returns>
		public static List<Province> ListOfProVinces()
		{
			return provinceDB.List().ToList();
		}

        public static List<StatusOrder> ListOfStatusOrders()
        {
            return statusDB.List().ToList();
        }
        /// <summary>
        ///  TIm kiem  va lay danh sach khach hang
        /// </summary>
        /// <param name="rowcount">Tham so dau vao  cho biet so dong du lieu tim duoc </param>
        /// <param name="page">trang can hien thi</param> 
        /// <param name="pagesize">so dong tren moi trang</param>
        /// <param name="searchValue">gia tri tim kiem ( rong voi toan bo khach hang)</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowcount, int page = 1, int pagesize = 0 , string searchValue = "")
		{
			rowcount = customerDB.Count(searchValue);
			return customerDB.List(page, pagesize, searchValue).ToList();
		}
        public static List<Customer> ListOfCustomers()
        {
            return customerDB.List().ToList();
        }
        /// <summary>
        /// Lay thong tin 1 khach hang theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
		{
			return customerDB.Get(id);
		}
		/// <summary>
		/// Bo sung 1 khach hang moi . Ham tra ve ma khach hang vua duoc bo sung
		/// (Ham tra ve -1 neu email bi trung	, tra ve 0 neu bi loi)
		/// </summary>
		/// <param name="customer"></param>
		/// <returns></returns>
		public static int AddCustomer(Customer customer)
		{
			return customerDB.Add(customer);
		}
		/// <summary>
		/// Cap nhat thong tin 1 khach hang 
		/// </summary>
		/// <param name="customer"></param>
		/// <returns></returns>
		public static bool UpdateCustomer(Customer customer)
		{
			return customerDB.Update(customer);
		}
		/// <summary>
		/// Xoa 1 khach hang (neu khach hang khong co du lieu lien quan)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static bool DeleteCustomer(int id)
		{
			if (customerDB.IsUsed(id)){
				return false;
			}
			return customerDB.Delete(id);
		}
		/// <summary>
		/// Kiem tra xem khach hang co du lieu lien quan hay khong
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public  static bool IsUsedCustomer(int id)
		{
			return customerDB.IsUsed(id);
		}
        /// <summary>
        /// Tim kiem va lay danh sach nha cung cap
        /// </summary>
        /// <param name="rowcount">Tham so dau vao  cho biet so dong du lieu tim duoc </param>
        /// <param name="page">trang can hien thi</param> 
        /// <param name="pagesize">so dong tren moi trang</param>
        /// <param name="searchValue">gia tri tim kiem ( rong voi toan bo khach hang)</param>
        /// <returns></returns>

        public static List<Supplier> ListOfSuppliers(out int rowcount, int page = 1, int pagesize = 0, string searchValue = "")
		{
            rowcount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pagesize, searchValue).ToList();
        }

        public static List<Supplier> ListOfSuppliers()
        {
            return supplierDB.List().ToList();
        }

        /// <summary>
        /// Lay thong tin 1 nha cung cap theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bo sung 1 nha cung cap  moi . Ham tra ve ma nha cung cap vua duoc bo sung
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }
        /// <summary>
        /// Cap nhat thong tin 1 nha cung cap 
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }
        /// <summary>
        /// Xoa 1 nha cung cap  (neu nha cung cap khong co du lieu lien quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.IsUsed(id))
            {
                return false;
            }
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// Kiem tra xem nha cung cap co du lieu lien quan hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }


        /// <summary>
        /// Tim kiem va lay danh sach shipper
        /// </summary>
        /// <param name="rowcount">Tham so dau vao  cho biet so dong du lieu tim duoc </param>
        /// <param name="page">trang can hien thi</param> 
        /// <param name="pagesize">so dong tren moi trang</param>
        /// <param name="searchValue">gia tri tim kiem ( rong voi toan bo khach hang)</param>
        /// <returns></returns>

        public static List<Shipper> ListOfShippers(out int rowcount, int page = 1, int pagesize = 0, string searchValue = "")
        {
            rowcount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pagesize, searchValue).ToList();
        }

        public static List<Shipper> ListOfShippers1()
        {
            return shipperDB.List().ToList();
        }

        /// <summary>
        /// Lay thong tin 1 shipper theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Bo sung 1 shipper  moi . Ham tra ve ma shipper vua duoc bo sung
        /// </summary>
        /// <param name="shipper"></param>
        /// <returns></returns>
        public static int AddShiper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }
        /// <summary>
        /// Cap nhat thong tin 1 shipper
        /// </summary>
        /// <param name="shipper"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        /// <summary>
        /// Xoa 1 shipper  (neu shipper khong co du lieu lien quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.IsUsed(id))
            {
                return false;
            }
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// Kiem tra xem shipper co du lieu lien quan hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }



        /// <summary>
        /// Tim kiem va lay danh sach nhan vien
        /// </summary>
        /// <param name="rowcount">Tham so dau vao  cho biet so dong du lieu tim duoc </param>
        /// <param name="page">trang can hien thi</param> 
        /// <param name="pagesize">so dong tren moi trang</param>
        /// <param name="searchValue">gia tri tim kiem ( rong voi toan bo khach hang)</param>
        /// <returns></returns>

        public static List<Employee> ListOfEmployees(out int rowcount, int page = 1, int pagesize = 0, string searchValue = "")
        {
            rowcount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pagesize, searchValue).ToList();
        }

        /// <summary>
        /// Lay thong tin 1 nhan vien theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Bo sung 1 nhan vien  moi . Ham tra ve ma nhan vien vua duoc bo sung
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee employee)
        {
            return employeeDB.Add(employee);
        }
        /// <summary>
        /// Cap nhat thong tin 1 nhan vien
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee employee)
        {
            return employeeDB.Update(employee);
        }
        /// <summary>
        /// Xoa 1 nhan vien  (neu nhan vien khong co du lieu lien quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
            {
                return false;
            }
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// Kiem tra xem nhan vien co du lieu lien quan hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }


        /// <summary>
        /// Tim kiem va lay danh sach danh muc san pham
        /// </summary>
        /// <param name="rowcount">Tham so dau vao  cho biet so dong du lieu tim duoc </param>
        /// <param name="page">trang can hien thi</param> 
        /// <param name="pagesize">so dong tren moi trang</param>
        /// <param name="searchValue">gia tri tim kiem ( rong voi toan bo khach hang)</param>
        /// <returns></returns>

        public static List<Category> ListOfCategories(out int rowcount, int page = 1, int pagesize = 0, string searchValue = "")
        {
            rowcount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pagesize, searchValue).ToList();
        }

        public static List<Category> ListOfCategories()
        {
            return categoryDB.List().ToList();
        }

        /// <summary>
        /// Lay thong tin 1 loai hang  theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bo sung 1 loai hang  moi . Ham tra ve ma loai hang vua duoc bo sung
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static int AddCategory(Category category)
        {
            return categoryDB.Add(category);
        }
        /// <summary>
        /// Cap nhat thong tin 1 loai hang
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category category)
        {
            return categoryDB.Update(category);
        }
        /// <summary>
        /// Xoa 1 loai hang  (neu loai hang khong co du lieu lien quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.IsUsed(id))
            {
                return false;
            }
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// Kiem tra xem loai hang co du lieu lien quan hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }
    }
}