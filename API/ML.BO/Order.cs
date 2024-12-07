using API.ML.BOBase;
using API.ML.Common;

namespace API.ML.BO
{
    public class Order : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid OrderID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid? CustomerID { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Tên order
        /// </summary>
        public string OrderName { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo order
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Tổng tiền order
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Trạng thái order
        /// </summary>
        public EnumOrderStatus Status { get; set; }

        /// <summary>
        /// Yêu cầu đặc biệt
        /// </summary>
        public string? SpecialRequest {  get; set; }

        /// <summary>
        /// Danh sách OrderDetail
        /// </summary>
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = [];

        /// <summary>
        /// Dữ liệu khách hàng
        /// </summary>
        public Customer? Customer { get; set; }
    }
}
