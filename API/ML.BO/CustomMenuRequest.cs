using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class CustomMenuRequest : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid CustomMenuRequestID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid CustomerID { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [SearchField]
        [StringLength(128)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Tên món
        /// </summary>
        [SearchField]
        [StringLength(128)]
        public string MenuItemName { get; set; } = string.Empty;

        /// <summary>
        /// Yêu cầu đặc biệt
        /// </summary>
        [StringLength(255)]
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public EnumCustomMenuRequestStatus Status { get; set; }

        /// <summary>
        /// Có lưu vào order không
        /// </summary>
        [NotMapped]
        public bool SaveToOrder { get; set; }

        /// <summary>
        /// Order để lưu món
        /// </summary>
        [NotMapped]
        public Guid? OrderID { get; set; }

        /// <summary>
        /// Có lưu vào thực đơn không
        /// </summary>
        [NotMapped]
        public bool SaveToMenu { get; set; }

        /// <summary>
        /// Dữ liệu khách hàng
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// Danh sách nguyên liệu
        /// </summary>
        [GetDetail]
        public List<InventoryItem>? InventoryItems { get; set; }
    }
}
