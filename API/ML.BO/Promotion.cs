using API.ML.Common;
using API.ML.CustomAtrributes;
using System.ComponentModel.DataAnnotations;

namespace API.ML.BO
{
    public class Promotion
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid PromotionID { get; set; }

        /// <summary>
        /// Mã khuyến mại
        /// </summary>
        [SearchField]
        public string PromotionCode { get; set; } = "";

        /// <summary>
        /// Tên khuyến mại
        /// </summary>
        [SearchField]
        public string PromotionName { get; set; } = "";

        /// <summary>
        /// Loại khuyến mại
        /// </summary>
        public EnumPromotionType PromotionType { get; set; }

        /// <summary>
        /// Cách tính khuyến mại
        /// 0: Theo phần trăm, 1: theo số tiền
        /// </summary>
        public EnumPromotionValueType ValueType { get; set; }

        /// <summary>
        /// Giá trị khuyến mại
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// ID món áp dụng khuyến mại
        /// </summary>
        public Guid? MenuItemID { get; set; }

        /// <summary>
        /// Dữ liệu món áp dụng khuyến mại
        /// </summary>
        public MenuItem? MenuItem { get; set; }
    }
}
