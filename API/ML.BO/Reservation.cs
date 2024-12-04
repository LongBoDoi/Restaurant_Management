using API.ML.BOBase;
using API.ML.Common;

namespace API.ML.BO
{
    public class Reservation : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        public Guid ReservationID { get; set; }

        /// <summary>
        /// ID khách hàng
        /// </summary>
        public Guid? CustomerID { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// SĐT khách hàng
        /// </summary>
        public string CustomerPhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian đặt bàn
        /// </summary>
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// Thông tin đặt bàn
        /// </summary>
        public string ReservationInfo { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái đặt bàn
        /// </summary>
        public EnumReservationStatus Status { get; set; }

        /// <summary>
        /// Số lượng khách
        /// </summary>
        public int GuestCount { get; set; }

        /// <summary>
        /// Yêu cầu đặc biệt
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu bản ghi khách hàng
        /// </summary>
        public Customer? Customer { get; set; }
    }
}
