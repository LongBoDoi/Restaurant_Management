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
    public enum EnumRole
    {
        /// <summary>
        /// Administrator
        /// </summary>
        Admin = 0,

        Manager = 1,

        Cashier = 2,

        Waiter = 3
    }

    public enum EnumMenuItemCategory
    {

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


}
