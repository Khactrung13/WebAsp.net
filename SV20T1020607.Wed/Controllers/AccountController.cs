using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020067.BusinessLayers;
using System.Text.RegularExpressions;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
           
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "" , string password = "")
        {
            ViewBag.Username = username;
            //Kiểm tra đầu vào

            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ thông tin!!!");
                return View();
            }
            //Kiểm tra thông tin đăng nhập có hợp lệ không
            var userAccount = UserAccountService.Authorize(username, password);
            if(userAccount == null)
            {
                ModelState.AddModelError("Error", "Tên đăng nhập hoặc mật khẩu không chính xác!!!");
                return View();
            }

            //Đăng nhập thành công -> Tạo dữ liệu để lưu cookie(WebUserData)
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName= userAccount.FullName,
                Email= userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //Thiết lập phiên đăng nhập cho tài khoản

            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword(string username)
        {
            ViewBag.username = username;
            return View();
        }
        [HttpPost]
        public IActionResult SavePassword(string username, string oldPassword, string newPassword1 , string newPassword)
        {
            ViewBag.username = username;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(oldPassword)
                || string.IsNullOrWhiteSpace(newPassword1) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ thông tin!!!");
                return View("ChangePassword");
            }
            
            var userAccount = UserAccountService.Authorize(username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Mật khẩu không chính xác!!!");
                return View("ChangePassword");
            }
            if (newPassword.Length < 6 || newPassword1.Length < 6)
            {
                ModelState.AddModelError("Error", "Mật khẩu phải có ít nhất 6 ký tự!!!");
                return View("ChangePassword");
            }
            if (!ContainsNoSpecialCharacters(newPassword) || !ContainsNoSpecialCharacters(newPassword1))
            {
                ModelState.AddModelError("Error", "Mật khẩu không được chứa các ký tự đặc biệt!!!");
                return View("ChangePassword");
            }
            if (newPassword1!= newPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu nhập lại không chính xác!!!");
                return View("ChangePassword");
            }
            if (oldPassword == newPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu mới trùng với mật khẩu cũ!!!");
                return View("ChangePassword");
            }
            bool resutl = UserAccountService.ChangePassword(username,oldPassword,newPassword);
            if (!resutl)
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

