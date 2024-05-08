using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Suppliers (SupplierName, ContactName, Provice, Address, Phone, Email,NgayThanhLap)
                            SELECT @SupplierName, @ContactName, @Provice, @Address, @Phone, @Email,@NgayThanhLap
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Suppliers WHERE  Email = @Email
                            );

                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    NgayThanhLap = data.NgayThanhLap
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = " ")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Suppliers 
                            WHERE (@searchValue = '') OR (SupplierName LIKE @searchValue)";

                var parameters = new { searchValue = searchValue };

                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
            }


            return count;
        }

        public bool Delete(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Supplier? Get(int id)
        {
            Supplier? supplier = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                //Thuc thi
                supplier = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return supplier;
        }

        public bool IsUsed(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM Products WHERE SupplierID = @SupplierID) AS Used";
                var parameters = new
                {
                    SupplierID = id
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return resutl;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> list = new List<Supplier>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                                SELECT 
                                    *,
                                    ROW_NUMBER() OVER (ORDER BY SupplierName) AS RowNumber
                                FROM 
                                    Suppliers
                                WHERE 
                                    (@searchValue = '' OR SupplierName LIKE @searchValue)
                            ) AS SubQuery
                            WHERE 
                                (@pageSize = 0 OR @page < 1) OR 
                                (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                            ORDER BY 
                                RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                list = connection.Query<Supplier>(sql, parameters, commandType: CommandType.Text).ToList();
            }
            return list;
        }

        public bool Update(Supplier data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Suppliers 
                            SET SupplierName = @SupplierName,
                                ContactName = @ContactName,
                                Provice = @Provice,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                NgayThanhLap = @NgayThanhLap
                            WHERE SupplierId = @SupplierId
                            AND NOT EXISTS(SELECT * FROM Suppliers WHERE SupplierId <> @SupplierId AND Email = @Email)";
                var parameters = new
                {
                    SupplierId = data.SupplierID,
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    NgayThanhLap = data.NgayThanhLap
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

