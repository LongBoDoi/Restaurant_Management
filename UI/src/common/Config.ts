class Config {
    // Có dùng cookies không
    static UseCookies:boolean = import.meta.env.VITE_USE_COOKIES === 'true';
};

export default Config;