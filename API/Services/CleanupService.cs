using API.ML.BO;
using API.ML.BOBase;
using Newtonsoft.Json;

namespace API.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Tính thời gian còn lại tới nửa đêm
                    TimeSpan timeUntilMidnight = GetTimeUntilMidnight();
                    Console.WriteLine($"Thực thi task dọn dẹp trong {timeUntilMidnight}...");

                    await Task.Delay(timeUntilMidnight, stoppingToken); // Chờ task đến 00:00

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                        List<string> activeUrls = this.GetAllActiveUrls(dbContext);

                        if (activeUrls.Any())
                        {
                            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                            if (Directory.Exists(uploadFolder))
                            {
                                var imageFiles = Directory.GetFiles(uploadFolder);
                                foreach (var file in imageFiles)
                                {
                                    if (!activeUrls.Contains(Path.GetFileName(file)))
                                    {
                                        System.IO.File.Delete(file);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        private TimeSpan GetTimeUntilMidnight()
        {
            DateTime now = DateTime.UtcNow;
            DateTime nextMidnight = now.Date.AddDays(1);
            return nextMidnight - now;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ link ảnh đang hoạt động trong DB
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllActiveUrls(ApplicationDBContext dbContext)
        {
            List<string> result = [];

            // Lấy thiết lập Ảnh giới thiệu nhà hàng
            Setting? introImageSetting = dbContext.Setting.FirstOrDefault(s => s.SettingKey == "IntroImageUrl");
            if (introImageSetting != null && !string.IsNullOrEmpty(introImageSetting.SettingValue))
            {
                result.Add(introImageSetting.SettingValue);
            }

            // Lấy thiết lập ảnh thực đơn cho khách hàng
            Setting? menuImageForCustomerSetting = dbContext.Setting.FirstOrDefault(s => s.SettingKey == "ListMenuScreenForCustomerImages");
            if (menuImageForCustomerSetting != null && !string.IsNullOrEmpty(menuImageForCustomerSetting.SettingValue))
            {
                List<string> lstMenuImageUrls = JsonConvert.DeserializeObject<List<string>>(menuImageForCustomerSetting.SettingValue) ?? [];
                result.AddRange(lstMenuImageUrls);
            }

            return result;
        }
    }
}
