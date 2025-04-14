/**
 * Enum mã lỗi chương trình
 */
export enum EnumApplicationErrorCode {
    /// Lỗi trùng thông tin đăng ký (SĐT, mã nhân viên)
    DuplicateLoginInfo = 4001
}

/**
 * Edit mode
 */
export enum EnumEditMode {
    Add = 1,

    Edit = 2,

    Delete = 3
}

/**
 * Enum kiểu dữ liệu
 */
export enum EnumDataType {
    String = 1,

    Integer = 2,

    Boolean = 3
}

export enum EnumUserType {
    Employee = 0,

    Customer = 1
}

/**
 * Enum người gửi tin nhắn trong chat bot
 */
export enum EnumChatbotSender {
    /// Bot
    Bot = 0,

    /// Người dùng
    User = 1
}

/**
 * Enum trạng thái đặt bàn
 */
export enum EnumReservationStatus {
    /// Đang chờ xác nhận
    Pending = 1,

    /// Đã xác nhận
    Confirmed = 2,

    /// Đã huỷ
    Canceled = 3,

    /// Đã nhận bàn
    Received = 4
}

/**
 * Enum trạng thái làm việc của nhân viên
 */
export enum EnumEmployeeWorkStatus {
    // Đang làm việc
    Active = 0,

    // Thử việc
    Probation = 1,

    // Nghỉ phép
    OnLeave = 2,

    // Nghỉ việc
    Terminated = 3
}

/**
 * Enum loại món
 */
export enum EnumMenuItemCategory {
    /// Khai vị
    Appetizers = 0,

    /// Món chính
    MainCourse = 1,

    /// Tráng miệng
    Dessert = 2,

    /// Đồ uống
    Drink = 3
}

/**
 * Enum trạng thái order
 */
export enum EnumOrderStatus {
    Active = 1,

    Paid = 2,

    Canceled = 3
}

/**
 * Enum ngày trong tuần
 */
export enum EnumDayOfWeek {
    Sunday = 0,

    Monday = 1,

    Tuesday = 2,

    Wednesday = 3,

    Thursday = 4,

    Friday = 5,

    Saturday = 6,

    Holiday = 7
}

export enum EnumPaymentMethod {
    Cash = 0,
    
    Transaction = 1,

    Card = 2
}

/**
 * Trạng thái bàn
 */
export enum EnumTableStatus {
    // Còn trống
    Available = 0,

    // Hết chỗ
    Occupied = 1,

    // Đã được đặt chỗ
    Reserved = 2
}

/**
 * Enum trạng thái yêu cầu tạo món custom
 */
export enum EnumCustomMenuRequestStatus {
    Pending = 0,

    Approved = 1,

    Rejected = 2
}

class EnumerationInterface {
    EnumUserType: typeof EnumUserType = EnumUserType;
    EnumChatbotSender: typeof EnumChatbotSender = EnumChatbotSender;
    EnumMenuItemCategory: typeof EnumMenuItemCategory = EnumMenuItemCategory;
    EnumEditMode: typeof EnumEditMode = EnumEditMode;
    EnumReservationStatus: typeof EnumReservationStatus = EnumReservationStatus;
    EnumRole: typeof EnumEmployeeWorkStatus = EnumEmployeeWorkStatus;
    EnumOrderStatus: typeof EnumOrderStatus = EnumOrderStatus;
    EnumDayOfWeek: typeof EnumDayOfWeek = EnumDayOfWeek;
    EnumEmployeeWorkStatus: typeof EnumEmployeeWorkStatus = EnumEmployeeWorkStatus;
    EnumPaymentMethod: typeof EnumPaymentMethod = EnumPaymentMethod;
    EnumTableStatus: typeof EnumTableStatus = EnumTableStatus;
    EnumCustomMenuRequestStatus: typeof EnumCustomMenuRequestStatus = EnumCustomMenuRequestStatus;
}

export default EnumerationInterface;