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
    Add = 0,

    Edit = 1,

    Delete = 2
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
    Pending = 0,

    /// Đang hoạt động
    Active = 1,

    /// Đã đóng
    Closed = 2
}

/**
 * Enum vai trò nhân viên
 */
export enum EnumRole {
    /// Administrator
    Admin = 0
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

class EnumerationInterface {
    EnumUserType: typeof EnumUserType = EnumUserType;
    EnumChatbotSender: typeof EnumChatbotSender = EnumChatbotSender;
    EnumMenuItemCategory: typeof EnumMenuItemCategory = EnumMenuItemCategory;
    EnumEditMode: typeof EnumEditMode = EnumEditMode;
    EnumReservationStatus: typeof EnumReservationStatus = EnumReservationStatus;
}

export default EnumerationInterface;