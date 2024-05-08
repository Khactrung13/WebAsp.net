using System;
namespace SV20T1020607.DomainModels
{
	//Khach hang
	public class Customer
	{
		public int CustomerID { get; set; }
        public String CustomerName { get; set; } = " ";
        public String ContactName { get; set; } = " ";
        public String Province { get; set; } = " ";
        public String Address { get; set; } = " ";
        public String Phone { get; set; } = " ";
        public String Email { get; set; } = " ";
        public Boolean IsLocked { get; set; } = false;
    }
}

