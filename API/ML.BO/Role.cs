using API.ML.BOBase;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Role : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid RoleID { get; set; }

        /// <summary>
        /// Mã vai trò
        /// </summary>
        [SearchField]
        [StringLength(25)]
        public required string RoleCode { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary>
        [SearchField]
        [StringLength(255)]
        public required string RoleName { get; set; }

        /// <summary>
        /// Danh sách nhân viên
        /// </summary>
        [GetDataPagingInclude]
        public IEnumerable<Employee>? Employees { get; set; }

        /// <summary>
        /// Danh sách quyền
        /// </summary>
        [GetDetail]
        public IEnumerable<Permission>? Permissions { get; set; }
    }
}
