using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using SV20T1020390.web.AppCodes;
using SV20T1020390.web.Models;
using System.Buffers;

namespace SV20T1020390.web.Controllers
{
    [Authorize (Roles =$"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung loại hàng";
        const string CATEGORY_SEARCH = "category_search";
        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCategories(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new Models.CategorySearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model); //dữ liệu truyền cho View có kiểu Models.CategorySearchResult;
        }*/

        public IActionResult Index()
        {
            // Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            // Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
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
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm 
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var model = CommonDataService.GetCategory(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                ModelState.AddModelError(nameof(model.CategoryName), "Tên loại hàng không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                ModelState.AddModelError(nameof(model.Description), "Vui lòng nhập mô tả");
            }

            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                ViewBag.Tite = model.CategoryID == 0 ? CREATE_TITLE : "Cập nhật thông tin loại hàng";
                return View("Edit", model);
            }

            if (model.CategoryID == 0)
            {
                int id = CommonDataService.AddCategory(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.CategoryName), "Tên loại hàng bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCategory(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được loại hàng . Có thể tên loại hàng bị trùng");
                    ViewBag.Title = "Cập nhật loại hàng";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
