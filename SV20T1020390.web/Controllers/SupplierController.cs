using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using SV20T1020390.web.AppCodes;
using SV20T1020390.web.Models;

namespace SV20T1020390.web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung nhà cung cấp";
        const string SUPPLIER_SEARCH = "supplier_search";
        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new Models.SupplierSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model); //dữ liệu truyền cho View có kiểu Models.SupplierSearchResult
        }*/

        public IActionResult Index()
        {
            // Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            // Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
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
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm 
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Supplier()
            {
                SupplierID = 0,
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var model = CommonDataService.GetSupplier(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Save(Supplier model)
        {
            if (string.IsNullOrWhiteSpace(model.ContactName))
            {
                ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.SupplierName))
            {
                ModelState.AddModelError(nameof(model.SupplierName), "Tên nhà cung cấp không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                ModelState.AddModelError(nameof(model.Address), "Địa chỉ không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Province))
            {
                ModelState.AddModelError(nameof(model.Province), "Vui lòng chọn tỉnh/thành");
            }

            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                ViewBag.Tite = model.SupplierID == 0 ? CREATE_TITLE : "Cập nhật thông tin nhà cung cấp";
                return View("Edit", model);
            }

            if (model.SupplierID == 0)
            {
                int id = CommonDataService.AddSupplier(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateSupplier(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng . Có thể email bị trùng");
                    ViewBag.Title = "Cập nhật nhà cung cấp";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
