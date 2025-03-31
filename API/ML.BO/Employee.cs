using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Employee : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [NameField]
        [StringLength(25)]
        public string EmployeeCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        [NameField]
        [StringLength(128)]
        public string EmployeeName { get; set; } = "";

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(50)]
        public string? PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại
        /// </summary>
        [StringLength(50)]
        public string? Email { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái làm việc
        /// </summary>
        public EnumEmployeeWorkStatus WorkStatus { get; set; }

        /// <summary>
        /// Ảnh đại diện
        /// </summary>
        [StringLength(128)]
        public string? ImageUrl { get; set; }

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
