using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
	public class CategoryDAL : BaseDAL , ICommonDAL<Category>
	{
		public CategoryDAL(string connectionString) : base(connectionString)
		{
		}

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Categories (CategoryName, Description)
                            SELECT @CategoryName,@Description
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Categories WHERE CategoryName = @CategoryName
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
 
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
                var sql = @"SELECT COUNT(*) FROM Categories 
                            WHERE (@searchValue = '') OR (CategoryName LIKE @searchValue)";

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
                var sql = @"delete from Categories where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Category? Get(int id)
        {
            Category? category = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id
                };
                //Thuc thi
                category = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return category;
        }

        public bool IsUsed(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM Products WHERE CategoryID = @CategoryID) AS Used";
                var parameters = new
                {
                    CategoryID = id
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return resutl;
        }

        public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> list = new List<Category>();

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
                                    ROW_NUMBER() OVER (ORDER BY CategoryName) AS RowNumber
                                FROM 
                                    Categories
                                WHERE 
                                    (@searchValue = '' OR CategoryName LIKE @searchValue)
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
                list = connection.Query<Category>(sql, parameters, commandType: CommandType.Text).ToList();
            }
            return list;
        }

        public bool Update(Category data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Categories 
                    SET CategoryName = @CategoryName,
                        Description = @Description
                    WHERE CategoryId = @CategoryId
                    AND NOT EXISTS(SELECT * FROM Categories WHERE CategoryId <> @CategoryId AND CategoryName = @CategoryName)";
                var parameters = new
                {
                    CategoryId = data.CategoryID,
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? ""
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

