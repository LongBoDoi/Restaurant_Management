using API.ML.Common;
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
        public string PromotionCode { get; set; } = "";

        /// <summary>
        /// Tên khuyến mại
        /// </summary>
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
    }
}
