using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020067.BusinessLayers;
using SV20T1020607.DomainModels;
using SV20T1020607.Wed.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator} ,{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        const string Create_title = "Bổ sung khách hàng";
        const string Update_title = "Cập nhật khách hàng";
        const string CUSTOMER_SEARCH = "customer_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize =PAGE_SIZE,
                    SearchValue= ""
                };
            }
            return View(input);
        }
        /// <summary>
        /// Tìm kiếm dựa trên đầu vào đã nhập trên Index và trả về kết quả 
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult()
            {
                Page = input.Page,
                PageSize=input.PageSize,
                SearchValue=input.SearchValue ?? "",
                RountCount= rowCount,
                Data= data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }



        public IActionResult Create()
        {
            ViewBag.Title = Create_title;
            
            var model = new Customer()
            {
                CustomerID = 0
            };

            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = Update_title;
            
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        public IActionResult Delete(int id= 0)
        {
            var model = CommonDataService.GetCustomer(id);
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCustomer(id);
             
                if (!result)
                {
                    ModelState.AddModelError("Error", "Xóa khách hàng không thành công");
                    ViewBag.Title = "Xóa khách hàng";
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
           
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost] //attribute => chi nhan du lieu gui len duoi dang post 
        public IActionResult Save(Customer model) // int customerId, string customerName,...
        {
            //Kiểm tra xem dữ liệu có hợp lệ không
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                ModelState.AddModelError(nameof(model.CustomerName),"Tên khách hàng không được để trống!!!");
            else if (!Regex.IsMatch(model.CustomerName, @"^[\p{L}\s]+$"))
                ModelState.AddModelError(nameof(model.CustomerName), "Tên khách hàng không hợp lệ!!!!");           
            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email không được để trống!!!");
            else if (!Regex.IsMatch(model.Email, @"^([\w\d_]+)@([a-zA-Z]+)((\.((com)|(vn)))+)$"))
                ModelState.AddModelError(nameof(model.Email), "Email không hợp lệ !!!");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống!!!");
            else if (!Regex.IsMatch(model.Phone, @"^\d{10}$"))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không hợp lệ!!!");
            if (string.IsNullOrWhiteSpace(model.Province))
                ModelState.AddModelError(nameof(model.Province), "Vui lòng chọn Tỉnh/Thành!!!");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CustomerID == 0 ? Create_title : Update_title;
                return View("Edit",model);
            }

            if (model.CustomerID == 0)
            {
                int id = CommonDataService.AddCustomer(model);
                
                if (id <= 0)
                {
                    ModelState.AddModelError("Error", "Email đã tồn tại!!!");
                    ViewBag.Title = Create_title;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCustomer(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng (Email có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }
        
    }
}

