using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020067.BusinessLayers;
using SV20T1020607.Wed.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeAccountController : Controller
    {
        const int PAGE_SIZE = 20;
        const string ACOUNT_SEARCH = "account_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(ACOUNT_SEARCH);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeAccountSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(ACOUNT_SEARCH, input);

            return View(model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.id = id;
            var data = CommonDataService.GetEmployee(id);
            return View(data);
        }
        public IActionResult SavePassword(int EmployeeId = 0 , string newPassword1="" ,string newPassword="")
        {
            ViewBag.id = EmployeeId;
            var data = CommonDataService.GetEmployee(EmployeeId);

            if (string.IsNullOrWhiteSpace(newPassword1) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ thông tin!!!");
                return View("Edit");
            }
            if (newPassword1 == data.Password)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới trùng với mật khẩu cũ!!!");
                return View("Edit");
            }
            if (newPassword.Length < 6 || newPassword1.Length < 6)
            {
                ModelState.AddModelError("Error", "Mật khẩu phải có ít nhất 6 ký tự!!!");
                return View("Edit");
            }
            if (!ContainsNoSpecialCharacters(newPassword) || !ContainsNoSpecialCharacters(newPassword1))
            {
                ModelState.AddModelError("Error", "Mật khẩu không được chứa các ký tự đặc biệt!!!");
                return View("Edit");
            }
            if (newPassword1 != newPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu nhập lại không chính xác!!!");
                return View("Edit");
            }
            bool result =  UserAccountService.ChangePassword(data.Email,data.Password, newPassword);
            if (!result)
            {
                ModelState.AddModelError("Error", "Đổi mật khẩu không thành công!!!");
                return View("AccessDenined");
            }
            ModelState.AddModelError("success", "Đổi mật khẩu thành công!!!");
            return View("AccessDenined");
        }
        public IActionResult AccessDenined()
        {
            ModelState.AddModelError("Error", "Tài khoản của bạn không có quyền truy cập vào chức năng quản lý nhân viên!!!");
            return View();
        }
        private bool ContainsNoSpecialCharacters(string password)
        {
            // Biểu thức chính quy để kiểm tra mật khẩu không chứa các ký tự đặc biệt
            Regex regex = new Regex("^[a-zA-Z0-9@]+$");
            return regex.IsMatch(password);
        }
    }
}

