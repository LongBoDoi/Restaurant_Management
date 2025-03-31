using API.ML.BOBase;
using API.ML.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class MenuItem : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid MenuItemID { get; set; }

        /// <summary>
        /// Tên món
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả món
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Giá món
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Nhóm món
        /// </summary>
        public Guid? MenuItemCategoryID { get; set; }

        /// <summary>
        /// Hết hàng
        /// </summary>
        public bool OutOfStock { get; set; }

        /// <summary>
        /// Link ảnh món
        /// </summary>
        [StringLength(128)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu order detail
        /// </summary>
        public List<OrderDetail>? OrderDetails { get; set; }

        /// <summary>
        /// Dữ liệu nhóm thực đơn
        /// </summary>
        [ForeignKey("MenuItemCategoryID")]
        public MenuItemCategory? MenuItemCategory { get; set; }

        /// <summary>
        /// Dữ liệu nguyên liệu
        /// </summary>
        public List<MenuItemInventoryItem>? MenuItemInventoryItems { get; set; }
    }
}
