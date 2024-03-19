using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;

namespace SV20T1020390.web
{
        public class WebUserData
        {
            public string? UserId { get; set; }
            public string? UserName { get; set; }
            public string? DisplayName { get; set; }
            public string? Email { get; set; }
            public string? Photo { get; set; }
            public string? ClientIP { get; set; }
            public string? SessionId { get; set; }
            public string? AdditionalData { get; set; }
            public List<string>? Roles { get; set; }

            /// <summary>
            /// Tạo danh sách người dùng dưới dạng các Claim
            /// </summary>
            private List<Claim> Claims
            {
                get
                {
                    List<Claim> claims = new List<Claim>()
                {
                    new Claim(nameof(UserId), UserId ?? ""),
                    new Claim(nameof(UserName), UserName ?? ""),
                    new Claim(nameof(DisplayName), DisplayName ?? ""),
                    new Claim(nameof(Email), Email ?? ""),
                    new Claim(nameof(Photo), Photo ?? ""),
                    new Claim(nameof(ClientIP), ClientIP ?? ""),
                    new Claim(nameof(SessionId), SessionId ?? ""),
                    new Claim(nameof(AdditionalData), AdditionalData ?? "")
                };
                    if (Roles != null)
                        foreach (var role in Roles)
                            claims.Add(new Claim(ClaimTypes.Role, role));
                    return claims;
                }
            }

            /// <summary>
            /// Tạo Pricipal dựa trên thông tin của người dùng
            /// </summary>
            /// <returns></returns>
            public ClaimsPrincipal CreatePrincipal()
            {
                var claimIdentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdentity);
                return claimPrincipal;
            }
        }

        /// <summary>
        /// Thông tin về nhóm/quyền
        /// </summary>
        public class WebUserRole
        {
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="name">Tên/ký hiệu nhóm/quyền</param>
            /// <param name="description">Mô tả</param>
            public WebUserRole(string name, string description)
            {
                Name = name;
                Description = description;
            }

            /// <summary>
            /// Tên/ký hiệu nhóm/quyền
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Mô tả
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// Danh sách các nhóm quyền sử dụng trong ứng dụng
        /// </summary>
        public class WebUserRoles
        {
            /// <summary>
            /// Lấy thông tin các Role dựa vào các hằng được định nghĩa trong lớp này
            /// </summary>
            public static List<WebUserRole> ListOfRoles
            {
                get
                {
                    List<WebUserRole> listOfRoles = new List<WebUserRole>();
                    Type type = typeof(WebUserRoles);
                    var listFields = type.GetFields(BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));
                    foreach (var role in listFields)
                    {
                        string? roleName = role.GetRawConstantValue() as string;
                        if (roleName != null)
                        {
                            DisplayAttribute? attribute = role.GetCustomAttribute<DisplayAttribute>();
                            if (attribute != null)
                                listOfRoles.Add(new WebUserRole(roleName, attribute.Name ?? roleName));
                            else
                                listOfRoles.Add(new WebUserRole(roleName, roleName));
                        }
                    }
                    return listOfRoles;
                }
            }

            //TODO: Định nghĩa các Role được sử dụng trong hệ thống tại đây

            [Display(Name = "Quản trị hệt thống")]
            public const string Administrator = "admin";

            [Display(Name = "Nhân viên")]
            public const string Employee = "employee";

            [Display(Name = "Khách hàng")]
            public const string Customer = "customer";
        }

        /// <summary>
        /// Extension các phương thức cho các đối tượng liên quan đến xác thực tài khoản người dùng
        /// </summary>
        public static class WebUserExtensions
        {
            public static WebUserData? GetUserData(this ClaimsPrincipal principal)
            {
                try
                {
                    if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                        return null;
                    var userData = new WebUserData();
                    userData.UserId = principal.FindFirstValue(nameof(userData.UserId));
                    userData.UserName = principal.FindFirstValue(nameof(userData.UserName));
                    userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName));
                    userData.Email = principal.FindFirstValue(nameof(userData.Email));
                    userData.Photo = principal.FindFirstValue(nameof(userData.Photo));
                    userData.ClientIP = principal.FindFirstValue(nameof(userData.ClientIP));
                    userData.SessionId = principal.FindFirstValue(nameof(userData.SessionId));
                    userData.AdditionalData = principal.FindFirstValue(nameof(userData.AdditionalData));
                    userData.Roles = new List<string>();
                    foreach (var claim in principal.FindAll(ClaimTypes.Role))
                    {
                        userData.Roles.Add(claim.Value);
                    }
                    return userData;
                }
                catch
                {
                    return null;
                }
            }
        }
    
}
