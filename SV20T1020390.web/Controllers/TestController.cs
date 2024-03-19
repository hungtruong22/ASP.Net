using Microsoft.AspNetCore.Mvc;
using SV20T1020390.web.Models;
using System.Globalization;

namespace SV20T1020390.web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                Name = "Truong Minh Hung",
                BirthDate = new DateTime(1999, 10, 28),
                Salary = 500.25m
            };
            return View(model);
        }

        public IActionResult Save(Models.Person model, string birthdateInput = "")
        {
            // Chuyển chuỗi birthdateInput thành giá trị ngày,
            // nếu hợp lệ thì mới dùng giá trị do người dùng nhập
            // Nếu không thì vẫn dùng giá trị cũ

            DateTime? d = null;
            try
            {
                d = DateTime.ParseExact(birthdateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }catch (Exception ex)
            {
                
            }
            if( d.HasValue ) // != null
            {
                model.BirthDate = d.Value;
            }
            return Json(model);
        }
    }
}
