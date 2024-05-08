using System;
using Dapper;
using SV20T1020607.DomainModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SV20T1020607.DataLayer.MySql
{
    public class ProvinceDAL : BaseDAL, ICommonDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Province data)
        {
            throw new NotImplementedException();
        }

        public int Count(string searchValue = " ")
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Province? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsUsed(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Province> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Province> list = new List<Province>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM Provinces";

                list = connection.Query<Province>(sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(Province data)
        {
            throw new NotImplementedException();
        }
    }
}

