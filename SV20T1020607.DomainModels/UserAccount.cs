using System;
namespace SV20T1020607.DomainModels
{
	public class UserAccount
	{
		public string UserID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Password { get; set; } = "";
        /// <summary>
        /// Chuỗi các quyền tài khoản được ngăn cách bằng dấu , 
        /// </summary>
        public string RoleNames { get; set; } = "";
    }
}

