using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using SV20T1020390.web.AppCodes;
using SV20T1020390.web.Models;
using System.Reflection;

namespace SV20T1020390.web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class OrderController : Controller
    {
        // Số dòng trên 1 trang khi hiển thị danh sách đơn hàng
        private const int ORDER_PAGE_SIZE = 20;
        // Tên biến session để lưu điều kiện tìm kiếm đơn hàng
        private const string ORDER_SEARCH = "order_search"; 
        // Số dòng trên một trang khi hiển thị danh sách mặt hàng cần tìm khi lập đơn hàng
        private const int PRODUCT_PAGE_SIZE = 5;
        // Tên biến session lưu điều kiện tìm kiếm mặt hàng khi lặp đơn hàng
        private const string PRODUCT_SEARCH = "product_search_for_sale";

        //Tên biến session dùng để lưu giỏ hàng
        private const string SHOPPING_CART = "shopping_cart";

        /// <summary>
        /// Giao diện tìm kiếm và hiển thị kết quả tìm kiếm đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = ORDER_PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                                    DateTime.Today.AddMonths(-1), DateTime.Today)
                };
            }
            return View(input);
        }

        public IActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, input.PageSize,
                input.Status, input.FromTime, input.ToTime, input.SearchValue ?? "");
            var model = new OrderSearchResult
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Status = input.Status,
                TimeRange = input.DateRange ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if(order == null)
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
        /// Chuyển đơn hàng sang trạng thái đã được duyệt
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Accept (int id = 0) 
        {
            bool result = OrderDataService.AcceptOrder(id);
            if(!result)
            {
                TempData["Message"] = "Không thể duyệt đơn hàng này";
            }
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái đã kết thúc
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if(!result)
            {
                TempData["Message"] = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            }
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái hủy
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể thực hiện thao tác hủy đối với đơn hàng này";
            }
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái bị từ chối
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể thực hiện thao tác từ chối đối với đơn hàng này";
            }
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này";
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao diện trang lập đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if(input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = "",
                };
            }
            return View(input);
        }
        
        /// <summary>
        /// Tìm kiếm mặt hàng để đưa vào giỏ hàng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult SearchProduct(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page,
                        input.PageSize, input.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }

        /// <summary>
        /// Lấy giỏ hàng hiện đang lưu trong session
        /// </summary>
        /// <returns></returns>
        private List<OrderDetail> GetShoppingCart()
        {
            // Giỏ hàng là danh sách các mặt hàng (OrderDetail) được chọn để bán trong đơn hàng
            //và được lưu trong session
            var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        /// <summary>
        /// Trang hiển thị danh sách các mặt hàng đang có trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }
        
        public IActionResult AddToCart(OrderDetail data) 
        {
            if(data.SalePrice <= 0 || data.Quantity <= 0)
            {
                return Json("Giá bán và số lượng không hợp lệ");
            }

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if(existsProduct == null) // nếu mặt hàng chưa có trong giỏ thì bổ sung thêm vào giỏ hàng
            {
                shoppingCart.Add(data);
            }
            else // Nếu mặt hàng đã có trong giỏ thì tăng số lượng và thay đổi giá bán
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");    
        }

        /// <summary>
        /// Xóa mặt hàng ra khỏi giỏ hàng
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
        /// Khởi tạo đơn hàng (Lập một đơn hàng mới)
        /// Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ
        /// hoặc việc tạo đơn hàng không thành công
        /// Ngược lại, hàm trả về mã của đơn hàng được tạo (là một giá trị số)
        /// </summary>
        /// <param name="customerID">Mã khách hàng</param>
        /// <param name="deliveryProvince">Tỉnh/Thành giao hàng</param>
        /// <param name="deliveryAddress">Địa chỉ giao hàng</param>
        /// <returns></returns>
        public IActionResult Init(int customerID = 0, string deliveryProvince = "",
                    string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if(shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống, không thể lập đơn hàng");
            }
            if(customerID <= 0 || string.IsNullOrWhiteSpace(deliveryProvince)
                || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin");
            }
            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderID = OrderDataService.InitOrder(employeeID, customerID,
                                        deliveryProvince, deliveryAddress, shoppingCart); 
            ClearCart();
            return Json(orderID);
        }

        /// <summary>
        /// Xóa tất cả mặt hàng trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        /// <summary>
        /// Giao diện để sửa đổi thông tin mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã mặt hàng</param>
        /// <returns></returns>
        
        [HttpGet]         
        public IActionResult Detail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }


        /// <summary>
        /// Cập nhật giá bán và số lượng của một mặt hàng được bán trong đơn hàng.
        /// Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ hoặc lỗi,
        /// hàm trả về chuỗi rỗng nếu thành công
        /// </summary>
        /// <param name="OrderID">Mã đơn hàng</param>
        /// <param name="productID">Mã mặt hàng</param>
        /// <param name="quantity">Số lượng</param>
        /// <param name="salePrice">Giá bán</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(int OrderID, int ProductID,
            int Quantity, decimal SalePrice)
        {
            if(Quantity <= 0)
            {
                return Json("Số lượng bán không hợp lệ");
            }
            if(SalePrice < 0)
            {
                return Json("Giá bán không hợp lệ");
            }

            bool result = OrderDataService.SaveOrderDetail(OrderID, ProductID, Quantity, SalePrice);
            if (!result)
            {
                /*return Json("Không được phép thay đổi thông tin của đơn hàng này");*/
                return Json("haha " + result + " hohoh");
            }
            return Json("");
        }

        /// <summary>
        /// Xóa mặt hàng ra khỏi đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã mặt hàng cần xóa</param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
            {
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";
            }
            return RedirectToAction("Details", new {id});
        }

        /// <summary>
        /// Giao diện để chọn người giao hàng cho đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }


        /// <summary>
        /// Ghi nhận người giao hàng cho đơn hàng và chuyển đơn hàng sang trạng thái đang giao hàng
        /// Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ hoặc lỗi,
        /// hàm trả về chuỗi rỗng nếu thành công
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if(shipperID <= 0)
            {
                return Json("Vui lòng chọn người giao hàng");
            }

            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
            {
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng");
            }
            return Json("");
        }

        [HttpPost]
        public IActionResult ChangeAddressAndProvince(int id = 0,
            string deliveryAddress = "", string deliveryProvince = "")
        {
            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                /*return Json("Vui lòng chọn địa chỉ giao hàng");*/
                TempData["Message"] = "Vui lòng chọn địa chỉ giao hàng";
            }
            if (string.IsNullOrWhiteSpace(deliveryProvince))
            {
                /*return Json("Vui lòng chọn địa chỉ giao hàng");*/
                TempData["Message"] = "Vui lòng chọn Tỉnh/Thành";
            }

            bool result = OrderDataService.ChangeAddressAndProvince(id, deliveryAddress, deliveryProvince);
            if (!result)
            {
                TempData["Message"] = "Đơn hàng không cho phép chuyển cho người giao hàng";
            }
            if (result)
            {
                TempData["Message"] = "Cập nhật thành công";
            }
            return RedirectToAction("Details", new {id = id});
        }

        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productID = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productID);
            return View(model);
        }

    }
}
