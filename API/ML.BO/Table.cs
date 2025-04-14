using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Table : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid TableID { get; set; }

        /// <summary>
        /// Tên bàn
        /// </summary>
        [SearchField]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Số chỗ ngồi
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public EnumTableStatus Status { get; set; }

        /// <summary>
        /// ID khu vực
        /// </summary>
        public Guid? AreaID { get; set; }

        /// <summary>
        /// Dữ liệu khu vực
        /// </summary>
        [GetAll]
        [GetDataPagingInclude]
        public Area? Area { get; set; }

        /// <summary>
        /// Dữ liệu đặt chỗ
        /// </summary>
        public IEnumerable<ReservationTable>? ReservationTables { get; set; }

        /// <summary>
        /// Dữ liệu các order của bàn
        /// </summary>
        public IEnumerable<OrderTable>? OrderTables { get; set; }
    }
}
