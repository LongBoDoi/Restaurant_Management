using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Permission : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Mã quyền
        /// </summary>
        [StringLength(25)]
        public required string PermissionCode { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary>
        [StringLength(255)]
        public required string PermissionName { get; set; }

        /// <summary>
        /// Danh sách vai trò
        /// </summary>
        public IEnumerable<Role>? Roles { get; set; }
    }
}
