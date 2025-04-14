using API.ML.BOBase;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class InventoryItemCategory : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid InventoryItemCategoryID { get; set; }

        /// <summary>
        /// Tên nhóm nguyên liệu
        /// </summary>
        [StringLength(128)]
        [SearchField]
        public string InventoryItemCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Ngừng hoạt động
        /// </summary>
        public bool Inactive { get; set; }

        /// <summary>
        /// Số lượng nguyên liệu trong nhóm
        /// </summary>
        [NotMapped]
        public int InventoryItemCount { get; set; }

        /// <summary>
        /// Bản ghi nguyên vật liệu trong nhóm
        /// </summary>
        public IEnumerable<InventoryItem>? InventoryItems { get; set; }
    }
}
