using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using SV20T1020390.web.AppCodes;
using SV20T1020390.web.Models;
using System.Buffers;

namespace SV20T1020390.web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class ShipperController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung người giao hàng";
        const string SHIPPER_SEARCH = "shipper_search";
        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new Models.ShipperSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model); //dữ liệu truyền cho View có kiểu Models.ShipperSearchResult
        }*/

        public IActionResult Index()
        {
            // Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            // Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm 
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Shipper()
            {
                ShipperID = 0,
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var model = CommonDataService.GetShipper(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Save(Shipper model)
        {

            if (string.IsNullOrWhiteSpace(model.ShipperName))
            {
                ModelState.AddModelError(nameof(model.ShipperName), "Tên người giao hàng không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống");
            }

            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                ViewBag.Tite = model.ShipperID == 0 ? CREATE_TITLE : "Cập nhật thông tin người giao hàng";
                return View("Edit", model);
            }

            if (model.ShipperID == 0)
            {
                int id = CommonDataService.AddShipper(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.Phone), "Số điện thoại bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateShipper(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được ngươi giao hàng . Có thể số điện thoại bị trùng");
                    ViewBag.Title = "Cập nhật người giao hàng";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetShipper(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
