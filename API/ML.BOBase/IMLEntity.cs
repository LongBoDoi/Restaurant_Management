namespace API.ML.BOBase
{
    public interface IMLEntity
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Ngày chỉnh sửa
        /// </summary>
        DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Hàm clone object
        /// </summary>
        /// <returns></returns>
        IMLEntity? Clone();
    }
}
