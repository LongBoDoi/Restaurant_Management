using API.ML.BOBase;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class InventoryItem : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid InventoryItemID { get; set; }

        /// <summary>
        /// Tên NVL
        /// </summary>
        [NameField]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng tồn kho
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Số lượng cảnh báo tồn kho
        /// </summary>
        public decimal WarningStockQuantity { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu thực đơn
        /// </summary>
        public IEnumerable<MenuItemInventoryItem> MenuItemInventoryItems { get; set; } = [];
    }
}
