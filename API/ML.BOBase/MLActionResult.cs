using API.ML.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.ML.BOBase
{
    public class MLActionResult
    {
        /// <summary>
        /// Có thành công không
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Thông tin lỗi
        /// </summary>
        public string? ErrorMsg { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public EnumApplicationErrorCode? ErrorCode { get; set; }
    }
}
