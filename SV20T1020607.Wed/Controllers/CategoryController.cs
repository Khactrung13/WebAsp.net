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
    public class CategoryController : Controller
    {
        const int PAGE_SIZE = 20;
        const string Create_title = "Bổ sung loại hàng";
        const string Update_title = "Cập nhật thông tin loại hàng";
        const string CATEGORY_SEARCH = "category_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        // GET: /<controller>/
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = Create_title;
            Category model = new Category()
            {
                CategoryID = 0
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = Update_title;

            Category model = CommonDataService.GetCategory(id);
            if(model==null)
                return RedirectToAction("Index");
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            Category model = CommonDataService.GetCategory(id);
            if (Request.Method == "POST")
            {
                bool result =CommonDataService.DeleteCategory(id);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Xóa lọai hàng không thành công");
                    ViewBag.Title = "Xóa loại hàng";
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
        [HttpPost]
        public IActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError(nameof(model.CategoryName), "Tên loại hàng không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError(nameof(model.Description), "Mô tả không được để trống!!!");
            else if (!Regex.IsMatch(model.CategoryName, @"^[\p{L}0-9\s]+$"))
            {
                ModelState.AddModelError(nameof(model.CategoryName), "Tên loại hàng không hợp lệ !!!!");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CategoryID == 0 ? Create_title : Update_title;
                return View("Edit", model);
            }
            if (model.CategoryID == 0)
            {
                int id =CommonDataService.AddCategory(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Error", "Tên loại hàng đã tồn tại!!!");
                    ViewBag.Title = Create_title;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCategory(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được loại hàng (Tên loại hàng có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Edit", model);
                }
                   
            }
            return RedirectToAction("Index");
        }
    }
}

