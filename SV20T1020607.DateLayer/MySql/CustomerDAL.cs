using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using SV20T1020607.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV20T1020607.DataLayer.MySql
{
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Customers (CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                            SELECT @CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Customers WHERE Email = @Email
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
               id = connection.ExecuteScalar<int>(sql, parameters);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Customers 
                            WHERE (@searchValue = '') OR (CustomerName LIKE @searchValue)";

                var parameters = new { searchValue = searchValue };

                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                //Thuc thi
                resutl= connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Customer? Get(int id)
        {
            Customer? customer = null; 
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                //Thuc thi
                customer = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return customer;
        }

        public bool IsUsed(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM Orders WHERE CustomerId = @CustomerId) AS Used";
                var parameters = new
                {
                    CustomerId = id
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql, parameters);
                connection.Close();
            }
            return resutl;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> list = new List<Customer>();

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
                                ROW_NUMBER() OVER (ORDER BY CustomerName) AS RowNumber
                            FROM 
                                Customers
                            WHERE 
                                (@searchValue = '' OR CustomerName LIKE @searchValue)
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
                list = connection.Query<Customer>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(Customer data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers 
                    SET CustomerName = @CustomerName,
                        ContactName = @ContactName,
                        Province = @Province,
                        Address = @Address,
                        Phone = @Phone,
                        Email = @Email,
                        IsLocked = @IsLocked
                    WHERE CustomerId = @CustomerId
                    AND NOT EXISTS(SELECT * FROM Customers WHERE CustomerId <> @CustomerId AND Email = @Email)";
                var parameters = new
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

