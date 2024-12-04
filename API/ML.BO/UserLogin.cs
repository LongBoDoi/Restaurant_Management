using API.ML.BOBase;

namespace API.ML.BO
{
    public class UserLogin : MLEntity
    {
        #region Properties
        /// <summary>
        /// Khoá chính
        /// </summary>
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
    }
}
