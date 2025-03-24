using API.Controllers;
using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;

namespace API.ML.Utility
{
    public class CommonFunction
    {
        public static void HandleException(Exception ex, MLActionResult result, ApplicationDBContext context)
        {
            string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
              ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            try
            {
                result.Success = false;

                if (env == "Development")
                {
                    result.ErrorMsg = ex.Message;
                }
                else if (env == "Production")
                {
                    result.ErrorMsg = "Có lỗi xảy ra trên hệ thống.";
                }
                //context.ErrorLog.Add(new ErrorLog()
                //{
                //    ErrorLogID = Guid.NewGuid(),
                //    UserID = Session.UserID,
                //    Message = ex.Message
                //});
                //context.SaveChanges();
            } catch { 
            }
        }

        /// <summary>
        /// Lưu ảnh
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string SaveImage(IFormFile image)
        {
            string imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            try
            {
                string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                string newFilePath = Path.Combine(imageDirectory, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                    return newFileName;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
