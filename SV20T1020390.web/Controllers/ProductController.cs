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
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung mặt hàng";
        const string PRODUCT_SEARCH = "product_search";
        /*public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, page, PAGE_SIZE, searchValue ?? "", 0, 0, 0, 0);
            var model = new Models.ProductSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }*/

        public IActionResult Index()
        {
            // Kiểm tra xem trong session có lưu điều kiện tìm kiếm không
            // Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
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
                };
            }

            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID, input.SupplierID, 0, 0);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                Data = data
            };

            // Lưu lại điều kiện tìm kiếm 
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            ViewBag.IsEdit = false;
            var model = new Product()
            {
                ProductID = 0,
                Photo = "qbv.png",
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin sản phẩm";
            ViewBag.IsEdit = true;
            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
            {
                model.Photo = "qbv.png";
            }
            return View(model);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Save(Product model, IFormFile? uploadPhoto = null)
        {
            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                ModelState.AddModelError(nameof(model.ProductName), "Tên mặt hàng không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.ProductDescription))
            {
                ModelState.AddModelError(nameof(model.ProductDescription), "Mô tả mặt hàng không được để trống");
            }
            if (model.SupplierID.ToString() == "0")
            {
                ModelState.AddModelError(nameof(model.SupplierID), "Tên nhà cung cấp không được để trống");
            }
            if (model.CategoryID.ToString() == "0") 
            {
                ModelState.AddModelError(nameof(model.CategoryID), "Tên loại hàng không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Unit))
            {
                ModelState.AddModelError(nameof(model.Unit), "Tên đơn vị không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Price.ToString()))
            {
                ModelState.AddModelError(nameof(model.Price), "Giá hàng không được để trống");
            }
            List<Product> list
                = ProductDataService.ListProducts("");
            foreach (Product item in list)
            {
                if (model.ProductName == item.ProductName && model.ProductID != item.ProductID)
                {
                    ModelState.AddModelError(nameof(model.ProductName), $"Tên sản phẩm '{model.ProductName}' đã tồn tại.");
                    break;
                }
            }
            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                ViewBag.IsEdit = model.ProductID == 0 ? false : true;
                ViewBag.Tite = model.ProductID == 0 ? CREATE_TITLE : "Cập nhật thông tin mặt hàng";
                return View("Edit", model);
            }
            // xử lý ảnh upload:
            // Nếu có ảnh được upload thì lưu ảnh lên server, gán tên file ảnh đã lưu cho model.Photo
            if (uploadPhoto != null)
            {
               /* string fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ff}_{uploadPhoto.FileName}";*/
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu trên server
                // đường dẫn đến file lưu trên server (ví dụ: D:\MyWeb\wwwroot\images\photo.png)
                /*string filePath = Path.Combine(@"D:\laptrinhweb\SV20T1020390\SV20T1020390.web\wwwroot\images\employees", fileName);*/
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                // lưu file lên Server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }

                // Gán tên file ảnh cho model.Photo
                model.Photo = fileName;
            }

            if (model.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.ProductName), "Tên mặt hàng bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng . Có thể tên mặt hàng bị trùng");
                    ViewBag.Title = "Cập nhật mặt hàng";
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto = null)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                ModelState.AddModelError(nameof(model.Description), "Mô tả ảnh mặt hàng không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString()))
            {
                model.DisplayOrder = 0;
            }
            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString())
                || model.DisplayOrder.ToString() == "0")
            {
                ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị ảnh mặt hàng không được = 0");
            }

            List<ProductPhoto> listPhotos
                = ProductDataService.ListPhotos(model.ProductID);
            foreach (ProductPhoto item in listPhotos)
            {
                if(model.DisplayOrder == item.DisplayOrder && model.PhotoID != item.PhotoID)
                {
                    ModelState.AddModelError(nameof(model.DisplayOrder), $"Thứ tự hiển thị {model.DisplayOrder} đã tồn tại.");
                    break;
                }
            }
            
            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                /*ViewBag.IsEdit = model.ProductID == 0 ? false : true;*/
                ViewBag.Tite = model.PhotoID == 0 ? CREATE_TITLE : "Cập nhật thông tin ảnh mặt hàng";
                return View("Photo", model);
            }

            // xử lý ảnh upload:
            // Nếu có ảnh được upload thì lưu ảnh lên server, gán tên file ảnh đã lưu cho model.Photo
            if (uploadPhoto != null)
            {
                /* string fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ff}_{uploadPhoto.FileName}";*/
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu trên server
                // đường dẫn đến file lưu trên server (ví dụ: D:\MyWeb\wwwroot\images\photo.png)
                /*string filePath = Path.Combine(@"D:\laptrinhweb\SV20T1020390\SV20T1020390.web\wwwroot\images\employees", fileName);*/
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\productphotos", fileName);

                // lưu file lên Server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }

                // Gán tên file ảnh cho model.Photo
                model.Photo = fileName;
                string[] tachChuoiPhoto = model.Photo.Split('_');
            }
            if (model.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.Photo), "Ảnh bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Photo", model);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được ảnh mặt hàng. Có thể ảnh bị trùng");
                    ViewBag.Title = "Cập nhật mặt hàng";
                    return View("Photo", model);
                }
            }
            return RedirectToAction("Edit", new {id = model.ProductID});
        }

        public IActionResult Photo(int id = 0, string method = "", long photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var model = new ProductPhoto()
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "notphoto.png",
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh của mặt hàng";
                    var model1 = ProductDataService.GetPhoto(photoId);
                    if (model1 == null)
                    {
                        return RedirectToAction("Index");
                    }
                    if (string.IsNullOrWhiteSpace(model1.Photo))
                    {
                        model1.Photo = "notphoto.png";
                    }
                    return View(model1);
                case "delete":
                    bool result = ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Attribute(int id = 0, string method = "", long attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var model = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    var model1 = ProductDataService.GetAttribute(attributeId);
                    if (model1 == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model1);
                case "delete":
                    // TODO: xóa ảnh có mã photoId (xóa trực tiếp, không cần phải xác nhận)
                    bool result = ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id }); // trả về giao diện edit
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult SaveAttribute(ProductAttribute model, IFormFile? uploadPhoto = null)
        {
            if (string.IsNullOrWhiteSpace(model.AttributeName))
            {
                ModelState.AddModelError(nameof(model.AttributeName),
                    "Tên thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.AttributeValue))
            {
                ModelState.AddModelError(nameof(model.AttributeValue),
                    "Giá trị thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString()))
            {
                model.DisplayOrder = 0;
            }
            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString())
                || model.DisplayOrder.ToString() == "0")
            {
                ModelState.AddModelError(nameof(model.DisplayOrder),
                    "Thứ tự hiển thị không được = 0");
            }

            List<ProductAttribute> listAttributes
                = ProductDataService.ListAttributes(model.ProductID);
            foreach (ProductAttribute item in listAttributes)
            {
                if (model.DisplayOrder == item.DisplayOrder && model.AttributeID != item.AttributeID)
                {
                    ModelState.AddModelError(nameof(model.DisplayOrder), $"Thứ tự hiển thị {model.DisplayOrder} đã tồn tại.");
                    break;
                }
            }

            if (!ModelState.IsValid) // trả về true nếu trong ModelSate không tồn tại lỗi và ngược lại 
            {
                /*ViewBag.IsEdit = model.ProductID == 0 ? false : true;*/
                ViewBag.Tite = model.ProductID == 0 ? CREATE_TITLE : "Cập nhật thuộc tính của mặt hàng";
                return View("Attribute", model);
            }

            if (model.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.AttributeName), "Tên thuộc tính bị trùng");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Attribute", model);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được thuộc tính . Có thể tên thuộc tính bị trùng");
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    return View("Attribute", model);
                }
            }
            return RedirectToAction("Edit", new {id = model.ProductID});
        }
    }
}
