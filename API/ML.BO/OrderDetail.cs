using API.ML.BOBase;

namespace API.ML.BO
{
    public class OrderDetail : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid OrderDetailID { get; set; }

        /// <summary>
        /// ID order
        /// </summary>
        public Guid OrderID { get; set; }

        /// <summary>
        /// ID món
        /// </summary>
        public Guid MenuItemID { get; set; }

        /// <summary>
        /// Số lượng món
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Giá món
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Note món
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// Dữ liệu món
        /// </summary>
        public MenuItem? MenuItem { get; set; }
    }
}
