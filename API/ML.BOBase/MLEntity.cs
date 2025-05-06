using API.ML.Common;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.ML.BOBase
{
    public class MLEntity : IMLEntity
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get ; set; }

        /// <summary>
        /// Ngày chỉnh sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Edit mode
        /// </summary>
        [NotMapped]
        public EnumEditMode? EditMode { get; set; }

        /// <summary>
        /// Clone đối tượng
        /// </summary>
        /// <returns></returns>
        public IMLEntity? Clone()
        {
            return JsonConvert.DeserializeObject<IMLEntity>(value: JsonConvert.SerializeObject(this));
        }
    }
}
