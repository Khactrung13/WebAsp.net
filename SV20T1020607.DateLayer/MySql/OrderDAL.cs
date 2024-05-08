using System;
using System.Data;
using Azure;
using System.Drawing.Printing;
using Dapper;
using Mysqlx.Crud;
using SV20T1020607.DomainModels;
namespace SV20T1020607.DataLayer.MySql
{
    public class OrderDAL : BaseDAL , IOrderDAL
    {
        public OrderDAL(string connectionString) : base(connectionString)
        {

        }

        public int Add(DomainModels.Order data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Orders(CustomerId, OrderTime,DeliveryProvince, DeliveryAddress,EmployeeID, Status)
                            values(@CustomerID, Now(),@DeliveryProvince, @DeliveryAddress,@EmployeeID, 1);
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    CustomerId = data.CustomerID,
                    //OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince ?? "",
                    DeliveryAddress = data.DeliveryAddress ??"",
                    EmployeeID = data.EmployeeID,
                    Status = data.Status,

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            int count = 0;

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                    FROM Orders AS o
                    LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                    LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                    LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                    WHERE (@Status = 0 OR o.Status = @Status)
                        AND (@FromTime IS NULL OR o.OrderTime >= @FromTime)
                        AND (@ToTime IS NULL OR o.OrderTime <= @ToTime)
                        AND (@SearchValue = ''
                            OR c.CustomerName LIKE @SearchValue
                            OR e.FullName LIKE @SearchValue
                            OR s.ShipperName LIKE @SearchValue)";

                var parameters = new { Status = status, FromTime = fromTime, ToTime = toTime, SearchValue = searchValue };

                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int orderID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from OrderDetails where OrderID = @OrderID;
                            delete from Orders where OrderID = @OrderID";
                var parameters = new
                {
                    OrderID = orderID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public bool DeleteDetail(int OrderID, int ProductID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from OrderDetails
                            where OrderID = @OrderID and ProductID = @ProductID";
                var parameters = new
                {
                    OrderID = OrderID,
                    ProductID = ProductID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public DomainModels.Order? Get(int orderID)
        {
            DomainModels.Order? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select o.*,c.CustomerName,
                            c.ContactName as CustomerContactName,
                            c.Address as CustomerAddress,
                            c.Phone as CustomerPhone,
                            c.Email as CustomerEmail,
                            e.FullName as EmployeeName,
                            s.ShipperName,
                            s.Phone as ShipperPhone
                            from Orders as o
                            left join Customers as c on o.CustomerID = c.CustomerID
                            left join Employees as e on o.EmployeeID = e.EmployeeID
                            left join Shippers as s on o.ShipperID = s.ShipperID
                            where o.OrderID = @OrderID";
                var parameters = new
                {
                    OrderID = orderID
                };
                //Thuc thi
                data = connection.QueryFirstOrDefault<DomainModels.Order>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public OrderDetail? GetDetail(int orderID, int productID)
        {
            DomainModels.OrderDetail ? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails as od
                            join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID and od.ProductID = @ProductID";
                var parameters = new
                {
                    OrderID = orderID,
                    ProductID = productID
                };
                //Thuc thi
                data = connection.QueryFirstOrDefault<DomainModels.OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<DomainModels.Order> List(int page = 1, int pageSize = 0, int status = 0,
            DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            List<DomainModels.Order> list = new List<DomainModels.Order>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT 
    o.*,
    c.CustomerName,
    c.ContactName AS CustomerContactName,
    c.Address AS CustomerAddress,
    c.Phone AS CustomerPhone,
    c.Email AS CustomerEmail,
    e.FullName AS EmployeeName,
    s.ShipperName,
    s.Phone AS ShipperPhone,
    (SELECT COUNT(*) FROM Orders o2 WHERE o2.OrderTime >= o.OrderTime) AS RowNumber
FROM 
    Orders o
LEFT JOIN 
    Customers c ON o.CustomerID = c.CustomerID
LEFT JOIN 
    Employees e ON o.EmployeeID = e.EmployeeID
LEFT JOIN 
    Shippers s ON o.ShipperID = s.ShipperID
WHERE 
    (@Status = 0 OR o.Status = @Status)
    AND (@FromTime IS NULL OR o.OrderTime >= @FromTime)
    AND (@ToTime IS NULL OR o.OrderTime <= @ToTime)
    AND (@SearchValue = '' OR c.CustomerName LIKE @SearchValue OR e.FullName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)
    AND 
    (
        @pageSize = 0 OR @page < 1 OR 
        ((SELECT COUNT(*) FROM Orders o2 WHERE o2.OrderTime >= o.OrderTime) BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
    )
ORDER BY 
    o.OrderTime DESC;

                    ";
                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    FromTime = fromTime,
                    ToTime = toTime,
                    searchValue = searchValue ?? ""
                };
                list = connection.Query<DomainModels.Order>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public IList<OrderDetail> ListDetails(int orderID)
        {
            List<DomainModels.OrderDetail> list = new List<DomainModels.OrderDetail>();

            using (var connection = OpenConnection())
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails as od
                            join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID";
                var parameters = new
                {
                    OrderID = orderID
                };
                list = connection.Query<DomainModels.OrderDetail>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, SalePrice)
                        SELECT * FROM (SELECT @OrderID, @ProductID, @Quantity, @SalePrice) AS tmp
                        WHERE NOT EXISTS (
                            SELECT 1 FROM OrderDetails 
                            WHERE OrderID = @OrderID AND ProductID = @ProductID
                        ) LIMIT 1;

                        UPDATE OrderDetails
                        SET Quantity = @Quantity, SalePrice = @SalePrice
                        WHERE OrderID = @OrderID AND ProductID = @ProductID;";
                var parameters = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                    Quantity = quantity,
                    SalePrice = salePrice
                };
                //Thuc thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool Update(DomainModels.Order data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Orders
                        set CustomerID = @CustomerID,
                            OrderTime = @OrderTime,
                            DeliveryProvince = @DeliveryProvince,
                            DeliveryAddress = @DeliveryAddress,
                            EmployeeID = @EmployeeID,
                            AcceptTime = @AcceptTime,
                            ShipperID = @ShipperID,
                            ShippedTime = @ShippedTime,
                            FinishedTime = @FinishedTime,
                            Status = @Status
                            where OrderID = @OrderID";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince ?? "",
                    DeliveryAddress = data.DeliveryAddress ?? "",
                    EmployeeID= data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                    OrderID = data.OrderID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

