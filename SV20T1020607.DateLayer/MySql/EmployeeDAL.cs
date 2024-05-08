using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
	public class EmployeeDAL : BaseDAL , ICommonDAL<Employee>
	{
		public EmployeeDAL(string connectionString) : base(connectionString)
		{
		}

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Employees (FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
                            SELECT @FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Employees WHERE Email = @Email
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking
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
                var sql = @"SELECT COUNT(*) FROM Employees 
                            WHERE (@searchValue = '') OR (FullName LIKE @searchValue)";

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
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Employee? Get(int id)
        {
            Employee? employee = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                //Thuc thi
                employee = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return employee;
        }

        public bool IsUsed(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM Orders WHERE EmployeeID = @EmployeeID) AS Used";
                var parameters = new
                {
                    EmployeeID = id
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return resutl;
        }

        public IList<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> list = new List<Employee>();

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
                                ROW_NUMBER() OVER (ORDER BY FullName) AS RowNumber
                            FROM 
                                Employees
                            WHERE 
                                (@searchValue = '' OR FullName LIKE @searchValue)
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
                list = connection.Query<Employee>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(Employee data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Employees 
                    SET FullName = @FullName,
                        BirthDate = @BirthDate,
                        Address = @Address,
                        Phone = @Phone,
                        Email = @Email,
                        Photo = @Photo,
                        IsWorking = @IsWorking
                    WHERE EmployeeId = @EmployeeId
                    AND NOT EXISTS(SELECT * FROM Employees WHERE EmployeeId <> @EmployeeId AND Email = @Email)";
                var parameters = new
                {
                    EmployeeId = data.EmployeeID,
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

