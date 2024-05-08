using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using SV20T1020067.BusinessLayers;
using SV20T1020607.DomainModels;
using SV20T1020607.Wed.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator} ,{WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        const int PAGE_SIZE = 20;
        const string Create_title = "Bổ sung nhà cung cấp";
        const string Update_title = "Cập nhật thông tin nhà cung cấp";
        const string SUPPLIER_SEARCH = "supplier_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
           
            ViewBag.Title =Create_title;
            var model = new Supplier()
            {
                SupplierID = 0,
                NgayThanhLap = new DateTime(1990, 1, 1),
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title =Update_title;
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa nhà cung cấp";
            var model = CommonDataService.GetSupplier(id);
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteSupplier(id);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Xóa nhà cung cấp  không thành công");
                    ViewBag.Title = "Xóa nhà cung cấp";
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
         
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Supplier model ,string NgayThanhLapInput ="")
        {
            if (string.IsNullOrWhiteSpace(model.SupplierName))
                ModelState.AddModelError(nameof(model.SupplierName), "Tên nhà cung cấp không được để trống!!!");
            else if (!Regex.IsMatch(model.SupplierName, @"^[\p{L}0-9\s]+$"))
            {
                ModelState.AddModelError(nameof(model.SupplierName), "Tên nhà cung cấp không hợp lệ !!!!");
            }
            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Provice))
                ModelState.AddModelError(nameof(model.Provice), "Tỉnh/Thành không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError(nameof(model.Address), "Tên địa chỉ không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống!!!");
            else if (!Regex.IsMatch(model.Phone, @"^\d{10}$"))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không hợp lệ!!!");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email không được để trống!!!");
            else if (!Regex.IsMatch(model.Email, @"^([\w\d_]+)@([a-zA-Z]+)((\.((com)|(vn)))+)$"))
                ModelState.AddModelError(nameof(model.Email), "Email không hợp lệ !!!");
            if (string.IsNullOrWhiteSpace(NgayThanhLapInput))
            {
                ModelState.AddModelError(nameof(model.NgayThanhLap), "Ngày thành lập không được để trống!!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.SupplierID == 0 ? Create_title : Update_title;
                return View("Edit", model);
            }

            DateTime? d = NgayThanhLapInput.ToDateTime();
            if (d.HasValue)
                model.NgayThanhLap = d.Value;
            else
            {
                ModelState.AddModelError(nameof(model.NgayThanhLap), "Ngày thành lập không hợp lệ!!!");
                return View("Edit", model);
            }

            if (model.SupplierID == 0)
            {
                int id = CommonDataService.AddSupplier(model);
               ;
                if (id <= 0)
                {
                    ModelState.AddModelError("Error", "Email đã tồn tại!!!");
                    ViewBag.Title = Create_title;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result =CommonDataService.UpdateSupplier(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được nhà cung cấp (Email có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
            
        }
    }
}

