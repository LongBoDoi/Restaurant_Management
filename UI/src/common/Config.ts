class Config {
    // Có dùng cookies không
    static UseCookies:boolean = import.meta.env.VITE_USE_COOKIES === 'true';

    // Có hiển thị thông tin lỗi lên UI không
    static ShowErrorOnToast:boolean = import.meta.env.VITE_SHOW_ERROR_ON_TOAST === 'true';
};

export default Config;