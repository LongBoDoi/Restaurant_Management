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

    public enum EnumTimeFilter
    {
        ByDay = 0,

        ByWeek = 1,

        ByMonth = 2,

        ByYear = 3,

        Custom = 4
    }

    /// <summary>
    /// Edit mode
    /// </summary>
    public enum EnumEditMode
    {
        Add = 1,

        Edit = 2,

        Delete = 3
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
        
        Paid = 2,

        Canceled = 3
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
        Pending = 1,

        /// <summary>
        /// Đã xác nhận
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Đã huỷ
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Đã nhận bàn
        /// </summary>
        Received = 4
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

    /// <summary>
    /// Enum hình thức thanh toán
    /// </summary>
    public enum EnumPaymentMethod
    {
        /// <summary>
        /// Tiền mặt
        /// </summary>
        Cash = 0,

        /// <summary>
        /// Chuyển khoản
        /// </summary>
        Transaction = 1,

        /// <summary>
        /// Thẻ
        /// </summary>
        Card = 2
    }

    public enum EnumTableStatus
    {
        /// <summary>
        /// Còn trống
        /// </summary>
        Available = 0,

        /// <summary>
        /// Hết chỗ
        /// </summary>
        Occupied = 1,

        /// <summary>
        /// Đã được đặt chỗ
        /// </summary>
        Reserved = 2
    }

    /// <summary>
    /// Enum trạng thái yêu cầu tạo món custom
    /// </summary>
    public enum EnumCustomMenuRequestStatus
    {
        Pending = 0,

        Approved = 1,

        Rejected = 2
    }

    /// <summary>
    /// Enum loại chương trình khuyến mại
    /// </summary>
    public enum EnumPromotionType
    {
        /// <summary>
        /// Giảm giá món
        /// </summary>
        ItemDiscount = 0,

        /// <summary>
        /// Giảm giá order
        /// </summary>
        OrderDiscount = 1
    }

    /// <summary>
    /// Enum cách tính giá trị khuyến mại
    /// </summary>
    public enum EnumPromotionValueType
    {
        /// <summary>
        /// Theo phần trăm
        /// </summary>
        ByPercent = 0,

        /// <summary>
        /// Theo số tiền
        /// </summary>
        ByAmount = 1
    }
}
