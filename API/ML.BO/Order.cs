using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Order : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid OrderID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid? CustomerID { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [SearchField]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo order
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Tổng tiền order
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Tiền hàng
        /// </summary>
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Tiền tip
        /// </summary>
        public decimal TipAmount { get; set; }

        /// <summary>
        /// Trạng thái order
        /// </summary>
        public EnumOrderStatus Status { get; set; }

        /// <summary>
        /// Số tiền đã trả
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        public EnumPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Yêu cầu đặc biệt
        /// </summary>
        public string? SpecialRequest {  get; set; }

        /// <summary>
        /// Tên bàn
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Danh sách OrderDetail
        /// </summary>
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        /// <summary>
        /// Dữ liệu khách hàng
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// Dữ liệu bàn trong order
        /// </summary>
        [GetDataPagingInclude]
        public IEnumerable<OrderTable> OrderTables { get; set; } = new List<OrderTable>();
    }
}
