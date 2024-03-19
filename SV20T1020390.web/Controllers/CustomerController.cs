using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using SV20T1020390.web.AppCodes;
using SV20T1020390.web.Models;

namespace SV20T1020390.web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung khách hàng";
        const string CUSTOMER_SEARCH = "customer_search"; // Tên biến session dùng để lưu lại điều kiện tìm kiếm

        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            // cách 1: dùng ViewBag

            *//* int pageSize = 20;
             int rowCount = 0;

             var data = CommonDataService.ListOfCustommers(out rowCount, page, pageSize, searchValue);

             ViewBag.Page = page;
             ViewBag.RowCount = rowCount;

             int pageCount = rowCount / pageSize;
             if(rowCount % pageSize > 0)
             {
                 pageCount += 1;
                 ViewBag.PageCount = pageCount;
             }

             return View(data);*//*

            // cách 2: dùng Model
            int rowCount = 0;
            var data = CommonDataService.ListOfCustommers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new Models.CustomerSearchResult() 
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model); //dữ liệu truyền cho View có kiểu Models.CustomerSearchResult
        }*/

        public IActionResult Index()
        {
            // Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            // Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            if(input == null)
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
            var data = CommonDataService.ListOfCustommers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm 
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Customer()
            {
                CustomerId = 0,
            }; 
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = CommonDataService.GetCustomer(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // cách 1: save tường minh
        /*public IActionResult Save(int customerID = 0, string customerName = "", string contactName = "",
                                    string phone = "", string email = "", string address = "", string province = "",
                                    bool isLockes = false)
        // nhập 50 trường => hàm này có đến 50 tham số => kinh hoàng => dùng model để nhận dữ liệu
        // đổi bằng IActionResult Save(Customer model){return Json(model);}
        {
            var model = new // dynamic object
            {
                customerID,
                customerName,
                contactName,
                phone,
                email,
                address,
                province,
                isLockes
            };
            return Json(model);
        }*/


        // cách 2 : save dùng model
        [HttpPost] // Attribute => chỉ nhận dữ liệu gửi lên dưới dạng POST
        public IActionResult Save(Customer model)
        {
            // Kiểm soát dữ liệu trong model xem có hợp lệ không
            // Yêu cầu : Tên khách hàng, Enail, Tên giao dịch và Tỉnh thành không dược để trống
            if(string.IsNullOrWhiteSpace(model.CustomerName))
            {
                ModelState.AddModelError(nameof(model.CustomerName), "Tên không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.ContactName))
            {
                ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Province))
            {
                ModelState.AddModelError(nameof(model.Province), "Vui lòng chọn tỉnh/thành");
            }
            if (string.IsNullOrWhiteSpace(model.Address))
            {
                ModelState.AddModelError(nameof(model.Address), "Vui lòng nhập địa chỉ");
            }
            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(nameof(model.Phone), "Vui lòng nhập số điện thoại");
            }

            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                ViewBag.Tite = model.CustomerId == 0 ? CREATE_TITLE: "Cập nhật thông tin khách hàng";
                return View("Edit", model);
            }

            if (model.CustomerId == 0)
            {
                int id = CommonDataService.AddCustomer(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model); 
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCustomer(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng . Có thể email bị trùng");
                    ViewBag.Title = "Cập nhật khách hàng";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCustomer(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
// ctrl + R + R : chọn tên giống để sửa (refactor)

