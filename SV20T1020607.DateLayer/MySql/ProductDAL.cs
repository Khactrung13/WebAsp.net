using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;
using Azure;
using System.Drawing.Printing;

namespace SV20T1020607.DataLayer.MySql
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Products (ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo,IsSelling)
                            SELECT @ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo ,@IsSelling
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM Products WHERE ProductName = @ProductName
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID ,
                    CategoryID = data.CategoryID ,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductAttributes (ProductID, AttributeName, AttributeValue, DisplayOrder)
                            SELECT @ProductID, @AttributeName, @AttributeValue, @DisplayOrder
                            FROM dual
                            WHERE NOT EXISTS (
                                SELECT * FROM ProductAttributes WHERE AttributeName = @AttributeName AND ProductID = @ProductID
                            );
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    ProductID = data.ProductID ,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                   
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO ProductPhotos (ProductID, Photo, Description, DisplayOrder,IsHidden)
                            VALUES(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
       
                            SELECT IF(ROW_COUNT() > 0, LAST_INSERT_ID(), 0) AS InsertedID;";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int CategoryID = 0, int SupplierID = 0, decimal MinPrice = 0, decimal MaxPrice = 0)
        {
            int count = 0;

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Products 
                            WHERE (@searchValue = '' OR ProductName LIKE @searchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)";

                var parameters = new {
                    searchValue = searchValue,
                    CategoryID= CategoryID,
                    SupplierID= SupplierID,
                    MinPrice= MinPrice,
                    MaxPrice= MaxPrice

                };

                count = connection.ExecuteScalar<int>(sql, parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delele(int ProductID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID;
                            delete from ProductAttributes where ProductID = @ProductID;
                            delete from ProductPhotos where ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = ProductID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public bool DeleteAttrbute(long AttributeID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = AttributeID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public bool DeletePhoTo(long PhotoID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = PhotoID
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public Product? Get(int ProductID)
        {
            Product? product = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = ProductID
                };
                //Thuc thi
                product = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return product;
        }

        public ProductAttribute? GetAttribute(long AttributeID)
        {
            ProductAttribute? ProductAttribute = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = AttributeID
                };
                //Thuc thi
                ProductAttribute = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return ProductAttribute;
        }

        public ProductPhoto? GetPhoto(long PhotoID)
        {
            ProductPhoto? ProductPhoto = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = PhotoID
                };
                //Thuc thi
                ProductPhoto = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return ProductPhoto;
        }

        public bool IsUsed(int ProductID)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID) AS Used";
                var parameters = new
                {
                    ProductID = ProductID
                };
                //Thuc thi
                resutl = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return resutl;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int CategoryID = 0,
            int SupplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> list = new List<Product>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                                SELECT *,
                                       ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber
                                FROM Products
                                WHERE (@searchValue = '' OR ProductName LIKE @searchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                            ) SubQuery
                            WHERE (@pageSize = 0 OR @page < 1)
                               OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                            ORDER BY RowNumber;";

                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    CategoryID = CategoryID,
                    SupplierID= SupplierID,
                    minPrice= minPrice,
                    maxPrice= maxPrice
                };
                list = connection.Query<Product>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public IList<ProductAttribute> ListAttributes(int ProductID)
        {
            List<ProductAttribute> list = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = ProductID,
                };
                list = connection.Query<ProductAttribute>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public IList<ProductPhoto> ListPhotos(int ProductID)
        {
            List<ProductPhoto> list = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID order by DisplayOrder ASC";
                var parameters = new
                {
                    ProductID = ProductID,
                };
                list = connection.Query<ProductPhoto>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(Product data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Products 
                    SET ProductName = @ProductName,
                        ProductDescription = @ProductDescription,
                        SupplierID = @SupplierID,
                        CategoryID = @CategoryID,
                        Unit = @Unit,
                        Price=@Price,
                        Photo = @Photo,
                        IsSelling = @IsSelling
                    WHERE ProductID = @ProductID
                    AND NOT EXISTS(SELECT * FROM Products WHERE ProductID <> @ProductID AND ProductName = @ProductName)";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID ,
                    CategoryID = data.CategoryID ,
                    Unit = data.Unit ?? "",
                    Price = data.Price ,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductAttributes 
                    SET ProductID = @ProductID,
                        AttributeName = @AttributeName,
                        AttributeValue = @AttributeValue,
                        DisplayOrder = @DisplayOrder
 
                    WHERE AttributeID = @AttributeID
                    AND NOT EXISTS(SELECT * FROM ProductAttributes WHERE AttributeID <> @AttributeID AND AttributeName = @AttributeName)";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID ,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                  
                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool resutl = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductPhotos 
                    SET ProductID = @ProductID,
                        Photo = @Photo,
                        Description = @Description,
                        DisplayOrder = @DisplayOrder,
                        IsHidden =@IsHidden
                    WHERE PhotoID = @PhotoID 
                   ";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,

                };
                //Thuc thi
                resutl = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return resutl;
        }
    }
}

