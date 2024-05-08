﻿using System;
namespace SV20T1020607.DomainModels
{
	public class Supplier
	{
		public int SupplierID { get; set; }
		public string SupplierName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string Provice { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime NgayThanhLap { get; set; }
    }
}

