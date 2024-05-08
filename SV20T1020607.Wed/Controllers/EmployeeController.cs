using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
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
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 20;
        const string Create_title = "Bổ sung nhân viên";
        const string Update_title = "Cập nhật thông tin nhân viên";
        const string EMPLOYEE_SEARCH = "employee_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            ViewBag.Title = Create_title;
            var model = new Employee()
            {
                EmployeeID = 0,
                Photo = "nophoto.png",
                BirthDate = new DateTime(1990, 1, 1),
                IsWorking = true
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = Update_title;
            ViewBag.IsEdit = true;
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }
        public IActionResult Delete(int id = 0)
        {
            var model = CommonDataService.GetEmployee(id);

            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteEmployee(id);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Xóa nhân viên không thành công");
                    ViewBag.Title = "Xóa nhân viên";
                    return View(model);
                }
                else
                {
                    //Xóa ảnh
                    if (model.Photo != "nophoto.png")
                    {
                        if (!string.IsNullOrEmpty(model.Photo))
                        {
                            var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", model.Photo);
                            if (System.IO.File.Exists(imagePathToDelete))
                            {
                                System.IO.File.Delete(imagePathToDelete);
                            }
                        }
                    }


                    return RedirectToAction("Index");
                }

            }

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Employee model, IFormFile? uploadPhoto, string BirthDateInput = "")
        {

            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError(nameof(model.FullName), "Tên nhân viên không được để trống!!!");
            else if (!Regex.IsMatch(model.FullName, @"^[\p{L}\s]+$"))
            {
                ModelState.AddModelError(nameof(model.FullName), "Tên nhân viên  không hợp lệ !!!!");
            }
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
            string[] formats = { "dd/MM/yyyy" };
            if (string.IsNullOrWhiteSpace(BirthDateInput))
            {
                ModelState.AddModelError(nameof(model.BirthDate), "Ngày sinh không được để trống!!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.EmployeeID == 0 ? Create_title : Update_title;
                return View("Edit", model);
            }
            //Xử lý ngày sinh
            DateTime? d = BirthDateInput.ToDateTime();
            if (d.HasValue)
                model.BirthDate = d.Value;
            else
            {
                ModelState.AddModelError(nameof(model.BirthDate), "Ngày sinh không hợp lệ!!!");
                return View("Edit", model);
            }


            //Xử lý ảnh : nếu có ảnh upload thì lưu ảnh lên sever , gán tên file ảnh cho model.photo
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                // Lấy tên của tệp ảnh
                var FileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                // Tạo đường dẫn đầy đủ cho tệp ảnh đích
                string imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/employees", FileName);
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", FileName);

                // Sao chép tệp ảnh từ tạm thời vị trí tải lên sang thư mục images/employees
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                // Lưu tên của ảnh vào model
                model.Photo = FileName;

            }
            if (model.EmployeeID == 0)
            {

                int id = CommonDataService.AddEmployee(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Error", "Email nhân viên đã tồn tại!!!");
                    ViewBag.Title = Create_title;
                    return View("Edit", model);
                }

            }
            else
            {
                if (uploadPhoto != null && uploadPhoto.Length > 0)
                {
                    //Xóa ảnh ban đầu
                    var existingEmployee = CommonDataService.GetEmployee(model.EmployeeID);
                    if (existingEmployee.Photo != "nophoto.png")
                    {
                        if (!string.IsNullOrEmpty(existingEmployee.Photo))
                        {
                            string imagePathToDelete = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/employees", existingEmployee.Photo);
                            //var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "employees", existingEmployee.Photo);
                            if (System.IO.File.Exists(imagePathToDelete))
                            {
                                System.IO.File.Delete(imagePathToDelete);
                            }
                        }
                    }

                }

                bool result = CommonDataService.UpdateEmployee(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được nhân viên (Email có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
