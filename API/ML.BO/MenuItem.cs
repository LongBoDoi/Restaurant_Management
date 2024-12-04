using API.ML.BOBase;
using API.ML.Common;

namespace API.ML.BO
{
    public class MenuItem : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
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
        /// Loại món
        /// </summary>
        public EnumMenuItemCategory Category { get; set; }

        /// <summary>
        /// Hết hàng
        /// </summary>
        public bool OutOfStock { get; set; }

        /// <summary>
        /// Dữ liệu order detail
        /// </summary>
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
    }
}
