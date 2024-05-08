using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
    public class StatusOrderDAL : BaseDAL, ICommonDAL<StatusOrder>
    {
        public StatusOrderDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(StatusOrder data)
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

        public StatusOrder? Get(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsUsed(int id)
        {
            throw new NotImplementedException();
        }

        public IList<StatusOrder> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<StatusOrder> list = new List<StatusOrder>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM OrderStatus";

                list = connection.Query<StatusOrder>(sql, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(StatusOrder data)
        {
            throw new NotImplementedException();
        }
    }
}

