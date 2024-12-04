using API.ML.BOBase;

namespace API.ML.BO
{
    public class InventoryItem : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid InventoryItemID { get; set; }

        /// <summary>
        /// Tên NVL
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng tồn kho
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng tối thiểu để gợi ý đặt hàng
        /// </summary>
        public int ReorderLevel { get; set; }
    }
}
