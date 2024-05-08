using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020067.BusinessLayers;

namespace SV20T1020607.Wed
{
	public static class SelectListHelper
	{
		public static List<SelectListItem> Provinces()
		{
			List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "---Chọn tỉnh thành---"
            });
            foreach (var item in CommonDataService.ListOfProVinces()) {
				
				list.Add(new SelectListItem()
				{
					Value = item.ProvinceName,
					Text = item.ProvinceName
				}) ;
			}
			return list;
		}

        public static List<SelectListItem> StatusOrder()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "---Chọn trạng thái---"
            });
            foreach (var item in CommonDataService.ListOfStatusOrders())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.Status.ToString(),
                    Text = item.Description
                });
            }
            return list;
        }


        public static List<SelectListItem> Categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                
                Value = "",
                Text = "---Tất cả---"
            });
            foreach (var item in CommonDataService.ListOfCategories())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }
        public static List<SelectListItem> SelectCategories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {

                Value = "",
                Text = "---Chọn loại hàng---"
            });
            foreach (var item in CommonDataService.ListOfCategories())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {

                Value = "",
                Text = "---Tất cả---"
            });
            foreach (var item in CommonDataService.ListOfSuppliers())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }
        public static List<SelectListItem> SelectSuppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {

                Value = "",
                Text = "---Chọn nhà cung cấp---"
            });
            foreach (var item in CommonDataService.ListOfSuppliers())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        public static List<SelectListItem> SelectShippers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {

                Value = "",
                Text = "---Chọn người giao hàng ---"
            });
            foreach (var item in CommonDataService.ListOfShippers1())
            {

                list.Add(new SelectListItem()
                {
                    Value = item.ShipperID.ToString(),
                    Text = item.ShipperName
                });
            }
            return list;
        }
    }
}

