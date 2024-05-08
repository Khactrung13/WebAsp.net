using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
    public class OrderController : Controller
    {
        const int PAGE_SIZE = 20;
        const string ORDER_SEARCH = "order_search";
        const int PRODUCT_PAGE_SIZE = 5;
        const string PRODUCT_SEARCH = "product_search_for_sale";
        const string SHOPPING_CART = "shopping_cart"; 
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            //Nếu có thì sử dụng điều kiện tìm kiếm , ngược lại thì tìm kiếm theo điều kiện mặt định
            Models.OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:yy/MM/yyyy} - {1:dd/MM/yyyy}",
                                                DateTime.Today.AddMonths(-39),
                                                DateTime.Today
                                                )
                    
                };
            }
            return View(input);
        }
        /// <summary>
        /// Tìm kiếm dựa trên đầu vào đã nhập trên Index và trả về kết quả 
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, PAGE_SIZE,
                                                    input.Status,input.FromTime,input.ToTime,input.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = input.Page,
                PageSize = PAGE_SIZE,
                SearchValue = input.SearchValue ?? "",
                Status = input.Status,
                TimeRange= input.DateRange,
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);

            return View(model);
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng theo id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id=0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }
        /// <summary>
        /// Xóa sản phẩm trong đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id , int productId)
        {
           
            bool resutl = OrderDataService.DeleteOrderDetail(id, productId);

            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (model != null)
            {
                OrderDataService.DeleteOrderDetail(id, productId);
                return View("Details", model);
            }
         
            if (!resutl)
            {
                ModelState.AddModelError("Error", "Xóa sản phẩm không thành công");
                
            }
            return View("Details", model);
        }
        /// <summary>
        /// Sửa thông tin sản phẩm trong đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id,productId);
            
            return View(data);
        }
        [HttpPost]
        public IActionResult UpdateDetail(int id = 0 , int ProductID = 0 , int Quantity = 0 , decimal SalePrice = 0)
        {

            if (Quantity <= 0 || SalePrice <= 0)
            {
                var order1 = OrderDataService.GetOrder(id);
                if (order1 == null)
                {
                    return RedirectToAction("Index");
                }
                var details1 = OrderDataService.ListOrderDetails(id);
                var model1 = new OrderDetailModel()
                {
                    Order = order1,
                    Details = details1
                };
                ModelState.AddModelError("Error", "Số lượng và giá sản phẩm phải lớn hơn 0 ");
                return View("Details", model1);
            }
             bool resutl = OrderDataService.SaveOrderDetail(id, ProductID, Quantity, SalePrice);
            
            

            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!resutl)
            {
                ModelState.AddModelError("Error", "Sửa sản phẩm không thành công");
                
            }
            return View("Details", model);
        }

        
        /// <summary>
        /// Duyệt đơn hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);

            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View("Details", model);
        }
        /// <summary>
        /// Chuyển giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Shipping(int id=0)
        {
            ViewBag.Id = id;
            var model = OrderDataService.GetOrder(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Shipping(int id = 0 , int shipperID = 0)
        {
            ViewBag.Id = id;
            if (shipperID == 0)
            {
                ModelState.AddModelError("Shipper", "Vui lòng chọn người giao hàng!!!");
                var order1 = OrderDataService.GetOrder(id);
                if (order1 == null)
                {
                    return RedirectToAction("Index");
                }
                var details1 = OrderDataService.ListOrderDetails(id);
                var model1 = new OrderDetailModel()
                {
                    Order = order1,
                    Details = details1
                };
                return View("Details", model1);
            }
            var data = OrderDataService.GetOrder(id);
            if(data!=null && data.ShipperID != shipperID)
            {
               bool resutl = OrderDataService.ShipOrder(id, shipperID);
                if (!resutl)
                {
                    ModelState.AddModelError("Error", "Chuyển giao hàng không thành công");
                }
            }
            
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View("Details", model);
        }
        /// <summary>
        /// Kết thúc đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            OrderDataService.FinishOrder(id);
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View("Details", model);
        }
        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            OrderDataService.CancelOrder(id);
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View("Details", model);
        }
        /// <summary>
        /// Từ chối đơn hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            OrderDataService.RejectOrder(id);
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View("Details", model);
        }
        /// <summary>
        /// Xóa đơn hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!result)
            {
                ModelState.AddModelError("Error", "Xóa đơn hàng không thành công");
                return View("Details", model);
            }
            return View("Index");
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if(input== null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
               
                };
            }
            return View(input);
        }

        public IActionResult SearchProduct(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, input.Page,input.PageSize,
                                                     input.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",             
                RountCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return View(model);
        }
        /// <summary>
        /// Lay gio hang hien dang luu trong ss
        /// </summary>
        /// <returns></returns>
        private List<OrderDetail> GetShoppingCart()
        {
            //Gio hang la danh sach cac mat hang duoc chon de ban trong don hang
            //va luu trong Session
            var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART,shoppingCart);
            }
            return shoppingCart ;
        }
        /// <summary>
        /// Trang hien thi danh sach cac mat hang co trong gio hang
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }
        /// <summary>
        /// Bo sung mat hang
        /// ham tra ve chuoi khac rong neu du lieu khong hop le va nguoc lai
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddToCart(OrderDetail data)
        {
            var shoppingcart = GetShoppingCart();
            var existsProdcut = shoppingcart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if (existsProdcut == null) // mat hang chua co trong gio -> bo sung 
            {
                shoppingcart.Add(data);
            }
            else // mat hang da ton tai trong gio hang 
            {
                existsProdcut.Quantity += data.Quantity;
                //existsProdcut.SalePrice += data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingcart);
            //var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            return Json("");
        }
        /// <summary>
        /// Xoa mat hang ra khoi gio 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if(index >= 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        /// <summary>
        /// Xoa gio hang 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CLearCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống , không thể lập đơn hàng");
            }
            if (customerID <= 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress)){
                return Json("Vui lòng nhập đầy đủ thông tin");
            }
            if (!Regex.IsMatch(deliveryAddress, @"^[\p{L}0-9;\/\-\s]+$"))
            {
                return Json("Địa chỉ không hợp lệ!!!");
            }
            int employeeId = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderId = OrderDataService.InitOrder(employeeId, customerID, deliveryProvince, deliveryAddress, shoppingCart);
            if (orderId <= 0)
            {
                return Json("Tạo đơn hàng không thành công !!!");
            }
            CLearCart();
            return Json(orderId);
        }

        public IActionResult GetProvinceByCustomerID(int customerID = 0)
        {
            var data = CommonDataService.GetCustomer(customerID);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return Json(data.Province);
        }

        public IActionResult UpdateAddress(int id =0)
        {
            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult SaveAddress(int id ,string DeliveryAddress, string DeliveryProvince)
        {
            if(string.IsNullOrWhiteSpace(DeliveryAddress) || string.IsNullOrWhiteSpace(DeliveryProvince))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin !!!");
                var order1 = OrderDataService.GetOrder(id);
                return View("UpdateAddress", order1);
            }
            if (!Regex.IsMatch(DeliveryAddress, @"^[\p{L}0-9;\/\-\s]+$"))
            {
                ModelState.AddModelError("Error", "Địa chỉ không hợp lệ!!!");
                var order1 = OrderDataService.GetOrder(id);
                return View("UpdateAddress", order1);
               
            }
            bool result = OrderDataService.UpdateAddress(id, DeliveryAddress, DeliveryProvince);
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            if (!result)
            {
                ModelState.AddModelError("Error", "Cập nhật thông tin nhận hàng không thành công!!");
               
            }
            return View("Details", model);

        }
    }
}

