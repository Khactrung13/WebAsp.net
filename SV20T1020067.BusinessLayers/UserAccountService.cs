using System;
using SV20T1020607.DataLayer;
using SV20T1020607.DataLayer.MySql;
using SV20T1020607.DomainModels;

namespace SV20T1020067.BusinessLayers
{
	public static class UserAccountService
	{
        private static readonly IUserAccountDAL UserDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static UserAccountService()
        {
            UserDB = new EmployeeAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(string userName, string password)
        {
            
            return UserDB.Authorize(userName, password);
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            
            return UserDB.ChangePassword(userName, oldPassword,newPassword);
        }

    }
}

