using System;
namespace SV20T1020607.DomainModels
{
	public class Employee
	{
		public int EmployeeID { get; set; }
		public string FullName { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Photo { get; set; } = "";
        public bool IsWorking { get; set; } = false;
    }
}

