using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020390.DomainModels
{
    public class UserAccount
    {
        /// <summary>
        /// ID tài khoản
        /// </summary>
        public string UserID { get; set; } = "";

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// Tên đầy đủ
        /// </summary>
        public string FullName { get; set; } = "";

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Đường dẫn file ảnh
        /// </summary>
        public string Photo { get; set; } = "";

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Chuỗi các quyền của tài khoản, phân tách bởi dấu phẩy
        /// </summary>
        public string RoleNames { get; set; } = "";
    }
}
