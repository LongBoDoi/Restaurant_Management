using API.ML.BOBase;
using API.ML.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BO
{
    public class Setting : MLEntity
    {
        /// <summary>
        /// Khoá chính
        /// </summary>
        [Key]
        public Guid SettingID { get; set; }

        /// <summary>
        /// Key thiết lập
        /// </summary>
        [Required]
        [StringLength(255)]
        public string SettingKey { get; set; } = String.Empty;

        /// <summary>
        /// Giá trị thiết lập
        /// </summary>
        public string SettingValue { get; set; } = String.Empty;

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        public EnumDataType DataType { get; set; } = EnumDataType.String;

        [NotMapped]
        public object Value {
            get {
                switch (this.DataType)
                {
                    case EnumDataType.Integer:
                        return int.Parse(this.SettingValue);
                    case EnumDataType.Boolean:
                        return bool.Parse(this.SettingValue);
                    default:
                        return this.SettingValue;
                }
            }
        }
    }
}
