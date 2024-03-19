using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;
using System.Reflection;

namespace SV20T1020390.web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;
            //Kiểm tra xem thông tin nhập có đủ không 
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đủ tên và mật khẩu");
                return View();
            }

            //Kiểm tra thông tin đăng nhập có hợp lệ hay không 
            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            //Đăng nhập thành công , tạo dữ liệu để lưu cookie
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()

            };

            //Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword(string userName = "", string oldPassword = "",
                                            string newPassword = "", string confirmPassword = "")
        {
            if (Request.Method == "POST")
            {
                var user = User.GetUserData();
                if (user != null)
                {
                    if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword)
                        || string.IsNullOrWhiteSpace(confirmPassword))
                    {
                        ModelState.AddModelError("Error", "Thông tin không đầy đủ");
                        return View();
                    }

                    if (oldPassword != UserAccountService.getPasswordByUserName(userName))
                    {
                        ModelState.AddModelError("Error", "Bạn nhập không đúng mật khẩu cũ");
                        return View();
                    }

                    if (newPassword.Equals(oldPassword))
                    {
                        ModelState.AddModelError("Error", "Mật khẩu mới không được trùng mật khẩu cũ");
                        return View();
                    }

                    if (newPassword.Equals(confirmPassword) == false)
                    {
                        ModelState.AddModelError("Error", "Bạn đã nhập sai xác nhận mật khẩu");
                        return View();
                    }

                    UserAccountService.ChangePassword(userName, oldPassword, newPassword);
                    return RedirectToAction("Logout");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
