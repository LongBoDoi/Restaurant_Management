namespace API.ML.Common
{
    /// <summary>
    /// Enum mã lỗi chương trình
    /// </summary>
    public enum EnumApplicationErrorCode
    {
        /// <summary>
        /// Lỗi trùng thông tin đăng nhập (SĐT, mã NV,...)
        /// </summary>
        DuplicateLoginInfo = 4001,

        /// <summary>
        /// Sai thông tin đăng nhập
        /// </summary>
        InvalidLoginInfo = 4002
    }

    /// <summary>
    /// Edit mode
    /// </summary>
    public enum EnumEditMode
    {
        Add = 0,

        Edit = 1,

        Delete = 2
    }

    /// <summary>
    /// Enum kiểu dữ liệu
    /// </summary>
    public enum EnumDataType
    {
        String = 1,

        Integer = 2,

        Boolean = 3
    }

    /// <summary>
    /// Enum loại người dùng
    /// </summary>
    public enum EnumUserType
    {
        Employee = 0,

        Customer = 1
    }

    /// <summary>
    /// Enum quyền nhân viên
    /// </summary>
    public enum EnumEmployeeWorkStatus
    {
        /// <summary>
        /// Đang làm việc
        /// </summary>
        Active = 0,

        /// <summary>
        /// Thử việc
        /// </summary>
        Probation = 1,

        /// <summary>
        /// Đang nghỉ phép
        /// </summary>
        OnLeave = 2,

        /// <summary>
        /// Nghỉ việc
        /// </summary>
        Terminated = 3
    }

    public enum EnumOrderStatus
    {
        Active = 1,
        
        Paid = 2
    }

    /// <summary>
    /// Enum người gửi tin nhắn chatbot
    /// </summary>
    public enum EnumChatbotSender
    {
        /// <summary>
        /// Bot
        /// </summary>
        Bot = 0,

        /// <summary>
        /// Người dùng
        /// </summary>
        User = 1
    }

    /// <summary>
    /// Enum trạng thái đặt bàn
    /// </summary>
    public enum EnumReservationStatus
    {
        /// <summary>
        /// Chờ xác nhận
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Đã xác nhận
        /// </summary>
        Active = 1,

        /// <summary>
        /// Đã đóng
        /// </summary>
        Closed = 2
    }

    /// <summary>
    /// Enum cách hiển thị màn thực đơn cho khách hàng
    /// </summary>
    public enum EnumDisplayMenuScreenForCustomerType
    {
        /// <summary>
        /// Theo ảnh
        /// </summary>
        ByImages = 0,

        /// <summary>
        /// Theo món thiết lập
        /// </summary>
        ByItems = 1
    }

    /// <summary>
    /// Enum cách hiển thị màn thực đơn theo món thiết lập cho khách hàng
    /// </summary>
    public enum EnumDisplayMenuScreenByItemsForCustomerType
    {
        /// <summary>
        /// Toàn bộ món
        /// </summary>
        All = 0,

        /// <summary>
        /// Tuỳ chọn
        /// </summary>
        Custom = 1
    }
}
