using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class UserLogin : MLEntity
    {
        #region Properties
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid UserLoginID { get; set; }

        /// <summary>
        /// ID nhân viên
        /// </summary>
        public Guid? EmployeeID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid? CustomerID { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Token đăng nhập
        /// </summary>
        public string Token { get; set; } = string.Empty;
        #endregion

        #region Foreign key properties
        /// <summary>
        /// Thông tin của nhân viên
        /// </summary>
        public Employee? Employee { get; set; }

        /// <summary>
        /// Thông tin của khách hàng
        /// </summary>
        public Customer? Customer { get; set; }
        #endregion

        [NotMapped]
        public bool IsChangePassword { get; set; }

        [NotMapped]
        public string OldPassword { get; set; } = string.Empty;
    }
}
