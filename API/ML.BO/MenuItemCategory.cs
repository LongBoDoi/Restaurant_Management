using API.ML.BOBase;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class MenuItemCategory : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid MenuItemCategoryID { get; set; }

        /// <summary>
        /// Tên nhóm thực đơn
        /// </summary>
        [StringLength(128)]
        [SearchField]

        public string MenuItemCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Diễn giải
        /// </summary>
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Ngừng hoạt động
        /// </summary>
        public bool Inactive { get; set; }

        /// <summary>
        /// Thứ tự sắp xếp
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Màu hiển thị
        /// </summary>
        [StringLength(25)]
        public string Color { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng món của thực đơn
        /// </summary>
        [NotMapped]
        public int ItemCount { get; set; }

        /// <summary>
        /// Danh sách món trong nhóm
        /// </summary>
        public List<MenuItem>? MenuItems { get; set; }
    }
}
