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
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator} ,{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string Create_title = "Bổ sung mặt hàng";
        const string Update_title = "Cập nhật thông tin mặt hàng";
        const string CreatePhoto_title = "Bổ sung ảnh cho  mặt hàng";
        const string UpdatePhoto_title = "Cập nhật ảnh cho mặt hàng";
        const string CreateAttribute_title = "Bổ sung thuộc tính cho mặt hàng";
        const string UpdateAttribute_title = "Cập nhật thuộc tính cho mặt hàng";
        const string PRODUCT_SEARCH = "product_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    minPrice = 0,
                    maxPrice = 0
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, input.Page, input.PageSize,
                input.SearchValue ?? "", input.CategoryID
            , input.SupplierID, input.minPrice, input.maxPrice);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                minPrice = input.minPrice,
                maxPrice = input.maxPrice,
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.IsEdit = false;
            ViewBag.Title = Create_title;
            var model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png",
                IsSelling= true
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = Update_title;
            ViewBag.IsEdit = true;
            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }
        public IActionResult Delete(int id=0)
        {
            ViewBag.Title = "Xóa mặt hàng";
            var model = ProductDataService.GetProduct(id);
            if (Request.Method == "POST")
            {
                bool resutl = ProductDataService.DeleteProduct(id);
                if (!resutl)
                {
                    ModelState.AddModelError("Error", "Xóa mặt hàng không thành công");
                    ViewBag.Title = "Xóa mặt hàng";
                    return View(model);
                }
                else
                {
                    //Xóa ảnh ban đầu
                    if (model.Photo != "nophoto.png")
                    {
                        if (!string.IsNullOrEmpty(model.Photo))
                        {

                            var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", model.Photo);
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
        public IActionResult Save(Product model, IFormFile? uploadPhoto)
        {
           
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError(nameof(model.ProductName), "Tên mặt hàng không được để trống!!!");
            if (string.IsNullOrWhiteSpace(model.Unit))
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống!!!");
            else if (!Regex.IsMatch(model.Unit, @"^[\p{L}\s]+$"))
            {
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính  không hợp lệ !!!!");
            }
            if (model.CategoryID == 0)
                ViewBag.CategoryID = "Loại hàng không được để trống!!!";
            if (model.SupplierID == 0)
                ViewBag.SupplierID = "Tên nhà cung cấp không được để trống!!!";
            if (model.Price <= 0)
                ModelState.AddModelError(nameof(model.Price), "Giá sản phẩm phải lớn hơn 0!!!");
            else if (!Regex.IsMatch(model.Price.ToString(), @"^\d+(\.\d+)?$"))
                ModelState.AddModelError(nameof(model.Price), "Giá sản phẩm không hợp lệ!!!");
                        
            if (!ModelState.IsValid)
            {
                //ViewBag.IsEdit = true;
                ViewBag.Title = model.ProductID == 0 ? Create_title : Update_title;
                return View("Edit", model);
            }

            //Xử lý ảnh : nếu có ảnh upload thì lưu ảnh lên sever , gán tên file ảnh cho model.photo
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                // Lấy tên của tệp ảnh
                var FileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                // Tạo đường dẫn đầy đủ cho tệp ảnh đích
                string imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/products", FileName);
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", FileName);

                // Sao chép tệp ảnh từ tạm thời vị trí tải lên sang thư mục images/employees
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                // Lưu tên của ảnh vào model
                model.Photo = FileName;

            }
            if (model.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Error", "Tên mặt hàng đã tồn tại!!!");
                    ViewBag.Title = Create_title;
                    return View("Edit", model);
                }
            }
            else
            {
                //ViewBag.IsEdit = true;
                if (uploadPhoto != null && uploadPhoto.Length > 0)
                {
                    //Xóa ảnh ban đầu
                    var existingProduct = ProductDataService.GetProduct(model.ProductID);
                    if (!string.IsNullOrEmpty(existingProduct.Photo))
                    {
                        string imagePathToDelete = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/products", existingProduct.Photo);

                        if (System.IO.File.Exists(imagePathToDelete))
                        {
                            System.IO.File.Delete(imagePathToDelete);
                        }
                    }
                }
                bool result = ProductDataService.UpdateProduct(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được mặt hàng (Tên mặt hàng có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Edit", model);
                }
                

            }
            return RedirectToAction("Index");
        }



        public IActionResult Photo(int id,string method, long photoID=0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = CreatePhoto_title;
                    var model = new ProductPhoto()
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.png",

                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = UpdatePhoto_title;
                    ViewBag.edit = true;
                    model = ProductDataService.GetPhoto(photoID);
                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    //TODO: Xóa ảnh có mã là photoId(Xóa trực tiếp,không cần xác nhận
                    var model1 = ProductDataService.GetPhoto(photoID);
                    if (model1 != null)
                    {
                        if (model1.Photo != "nophoto.png")
                        {
                            if (!string.IsNullOrEmpty(model1.Photo))
                            {
                              
                                var imagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "PhotoProducts", model1.Photo);
                                if (System.IO.File.Exists(imagePathToDelete))
                                {
                                    System.IO.File.Delete(imagePathToDelete);
                                }
                            }
                        }
                        ProductDataService.DeletePhoto(photoID);
                        
                    }
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto ,int id)
        {
            //Them anh
            data.ProductID = id;
            var model = ProductDataService.GetProduct(id);
            ViewBag.IsEdit = true;
            if (data.DisplayOrder <=0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ!!");
            if (!ModelState.IsValid)
            {
                //ViewBag.IsEdit = true;
                ViewBag.Title = data.PhotoID == 0 ? CreatePhoto_title : UpdatePhoto_title;
                return View("Photo", data);
            }
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                // Lấy tên của tệp ảnh
                var FileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                
                    
                // Tạo đường dẫn đầy đủ cho tệp ảnh đích
                string imagePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/PhotoProducts", FileName);
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", FileName);

                // Sao chép tệp ảnh từ tạm thời vị trí tải lên sang thư mục images/employees
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                // Lưu tên của ảnh vào model
                data.Photo = FileName;

            }
           

            if (data.PhotoID == 0)
            {

                if (uploadPhoto == null)
                {
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh!!");
                    return View("Photo", data);
                }
                long idphoto =ProductDataService.AddPhoto(data);
                if (idphoto <= 0)
                {
                    ModelState.AddModelError("Error", "Thêm ảnh không thành công!!!");
                    ViewBag.Title = Create_title;
                    return View("Photo", data);
                }

            }
            else
            {
                if (uploadPhoto != null && uploadPhoto.Length > 0)
                {
                    //Xóa ảnh ban đầu
                    var existingProduct = ProductDataService.GetProduct(model.ProductID);
                    if (!string.IsNullOrEmpty(existingProduct.Photo))
                    {
                        string imagePathToDelete = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images/PhotoProducts", existingProduct.Photo);

                        if (System.IO.File.Exists(imagePathToDelete))
                        {
                            System.IO.File.Delete(imagePathToDelete);
                        }
                    }
                }
                bool result = ProductDataService.UppdatePhoto(data);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được ảnh (Thứ tự hiện thị có thể đã tồn tại)");
                    ViewBag.Title = Update_title;
                    return View("Photo", data);
                }
            }
            
            return View("Edit",model);
        }
        public IActionResult Attribute(int id, string method, long attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = CreateAttribute_title;
                    var model = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = id
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = UpdateAttribute_title;
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    //TODO: Xóa ảnh có mã là photoId(Xóa trực tiếp,không cần xác nhận
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult SaveAttribute(ProductAttribute data ,int id, string method, long attributeId = 0)
        {
            data.ProductID = id;
            var model = ProductDataService.GetProduct(id);
            ViewBag.IsEdit = true;
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống!!!");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống!!!");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải lớn 0!!!");

            if (!ModelState.IsValid)
            {
                //ViewBag.IsEdit = true;
                ViewBag.Title = data.ProductID == 0 ? CreateAttribute_title : UpdateAttribute_title;
                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long  i = ProductDataService.AddAttribute(data);
                if (i <= 0)
                {
                    ModelState.AddModelError("Error", "Tên thuộc tính đã tồn tại!!!");
                    ViewBag.Title = CreateAttribute_title;
                    return View("Attribute", data);
                }
            }
            else
            {
                bool result = ProductDataService.UppdateAttribue(data);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Tên thuộc tính đã tồn tại!!!");
                    ViewBag.Title = UpdateAttribute_title;
                    return View("Attribute", data);
                }
            }
            return View("Edit", model);
        }
    }

}

