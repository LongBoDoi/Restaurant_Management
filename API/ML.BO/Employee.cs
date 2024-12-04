using API.ML.BOBase;
using API.ML.Common;

namespace API.ML.BO
{
    public class Employee : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        public string EmployeeCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        public string EmployeeName { get; set; } = "";

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Vai trò
        /// </summary>
        public EnumRole Role { get; set; }

        /// <summary>
        /// Lịch làm việc
        /// </summary>
        public string? Schedule {  get; set; }

        /// <summary>
        /// Dữ liệu đăng nhập
        /// </summary>
        public UserLogin? UserLogin { get; set; }
    }
}
