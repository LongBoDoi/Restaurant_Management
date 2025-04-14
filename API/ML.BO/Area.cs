using API.ML.BOBase;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class Area : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid AreaID { get; set; }

        /// <summary>
        /// Tên khu vực
        /// </summary>
        [SearchField]
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng bàn
        /// </summary>
        [NotMapped]
        public int TableCount { get; set; }

        /// <summary>
        /// Danh sách bàn
        /// </summary>
        public IEnumerable<Table>? Tables { get; set; }
    }
}
