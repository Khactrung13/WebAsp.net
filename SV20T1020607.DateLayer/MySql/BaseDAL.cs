using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
	//Lop cha cua cac lop cai dat cac phep xu ly du lieu tren SQl
	public abstract class BaseDAL
	{
		protected string _connectionString;

		//Ctor
		public BaseDAL(String connectionString)
		{
			_connectionString = connectionString;

        }
		//Tao va mo ket noi den CSDL	
		protected MySqlConnection OpenConnection()
		{
		

            MySqlConnection connection = new MySqlConnection(_connectionString);
            //connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
	}

  

}

