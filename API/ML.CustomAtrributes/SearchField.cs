namespace API.ML.CustomAtrributes
{
    /// <summary>
    /// Trường sử dụng cho mục đích tìm kiếm theo phân trang
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchField : Attribute
    {
    }
}
