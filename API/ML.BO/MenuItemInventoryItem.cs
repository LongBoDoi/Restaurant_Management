namespace API.ML.BO
{
    public class MenuItemInventoryItem
    {
        /// <summary>
        /// ID thực đơn
        /// </summary>
        public Guid MenuItemID { get; set; }

        /// <summary>
        /// Dữ liệu thực đơn
        /// </summary>
        public MenuItem? MenuItem { get; set; }

        /// <summary>
        /// ID nguyên liệu
        /// </summary>
        public Guid InventoryItemID { get; set; }

        /// <summary>
        /// Dữ liệu nguyên liệu
        /// </summary>
        public InventoryItem? InventoryItem { get; set; }

        /// <summary>
        /// Số lượng nguyên liệu
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
