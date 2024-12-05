namespace API.ML.Common
{
    public static class Config
    {
        public static bool UseCookie { get; set; }

        public static void ReadConfig(WebApplicationBuilder builder)
        {
            bool v = bool.TryParse(builder.Configuration["AppSettings:UseCookies"], result: out bool useCookie);
            if (v)
            {
                UseCookie = useCookie;
            }
        }
    }
}
