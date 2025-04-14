using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class ReservationTable : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid ReservationTableID { get; set; }

        /// <summary>
        /// ID đặt chỗ
        /// </summary>
        public Guid ReservationID { get; set; }

        /// <summary>
        /// ID bàn
        /// </summary>
        public Guid TableID { get; set; }

        /// <summary>
        /// Dữ liệu đặt chỗ
        /// </summary>
        public Reservation? Reservation { get; set; }

        /// <summary>
        /// Dữ liệu bàn
        /// </summary>
        public Table? Table { get; set; }
    }
}
