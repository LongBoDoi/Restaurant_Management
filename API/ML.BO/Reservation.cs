using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Reservation : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid ReservationID { get; set; }

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
        /// SĐT khách hàng
        /// </summary>
        [SearchField]
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
        /// Tên bàn
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu bản ghi khách hàng
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// Dữ liệu bàn
        /// </summary>
        [GetDetail]
        [GetDataPagingInclude]
        public IEnumerable<ReservationTable> ReservationTables { get; set; } = new List<ReservationTable>();
    }
}
