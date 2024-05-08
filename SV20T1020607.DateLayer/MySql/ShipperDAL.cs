using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
	public class ShipperDAL : BaseDAL , ICommonDAL<Shipper>
	{
		public ShipperDAL(string connectionString) : base(connectionString)
		{
		}

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Shippers (ShipperName, Phone)
                            SELECT @ShipperName,@Phone
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Shippers WHERE Phone = @Phone
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
       
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
                var sql = @"SELECT COUNT(*) FROM Shippers 
                            WHERE (@searchValue = '') OR (ShipperName LIKE @searchValue)";

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
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Shipper? Get(int id)
        {
            Shipper? shipper = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id
                };
                //Thuc thi
                shipper = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return shipper;
        }

        public bool IsUsed(int id)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM Orders WHERE ShipperID = @ShipperID) AS Used";
                var parameters = new
                {
                    ShipperID = id
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return resutl;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> list = new List<Shipper>();

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
                                    ROW_NUMBER() OVER (ORDER BY ShipperName) AS RowNumber
                                FROM 
                                    Shippers
                                WHERE 
                                    (@searchValue = '' OR ShipperName LIKE @searchValue)
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
                list = connection.Query<Shipper>(sql, parameters, commandType: CommandType.Text).ToList();
            }
            return list;
        }


        public bool Update(Shipper data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Shippers 
                    SET ShipperName = @ShipperName,
                        Phone = @Phone
                    WHERE ShipperId = @ShipperId
                    AND NOT EXISTS(SELECT * FROM Shippers WHERE ShipperId <> @ShipperId AND Phone = @Phone)";
                var parameters = new
                {
                    ShipperId = data.ShipperID,
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

