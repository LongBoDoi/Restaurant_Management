using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [StringLength(128)]
        [SearchField]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả món
        /// </summary>
        [StringLength(255)]
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
        public bool Inactive { get; set; }

        /// <summary>
        /// Link ảnh món
        /// </summary>
        [StringLength(128)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu order detail
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        /// <summary>
        /// Dữ liệu nhóm thực đơn
        /// </summary>
        [ForeignKey("MenuItemCategoryID")]
        [GetDataPagingInclude]
        public MenuItemCategory? MenuItemCategory { get; set; }

        /// <summary>
        /// Dữ liệu nguyên liệu
        /// </summary>
        public List<MenuItemInventoryItem>? MenuItemInventoryItems { get; set; }
    }
}
