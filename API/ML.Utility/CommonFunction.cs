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
            result.Success = false;
            result.ErrorMsg = ex.Message;

            try
            {
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
    }
}
