using System;
using SV20T1020607.DomainModels;

namespace SV20T1020607.Wed.Models
{
	public class OrderDetailModel
	{
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}

