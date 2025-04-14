using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class OrderTable : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid OrderTableID { get; set; }

        /// <summary>
        /// ID Order
        /// </summary>
        public Guid OrderID { get; set; }

        /// <summary>
        /// ID bàn
        /// </summary>
        public Guid TableID { get; set; }

        /// <summary>
        /// Dữ liệu order
        /// </summary>
        public Order? Order { get; set; }

        /// <summary>
        /// Dữ liệu bàn
        /// </summary>
        public Table? Table { get; set; }
    }
}
