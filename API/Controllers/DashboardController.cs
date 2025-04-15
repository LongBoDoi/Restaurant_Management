using API.ML.BOBase;
using API.ML.CustomAtrributes;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController(ApplicationDBContext context)
    {
        private readonly ApplicationDBContext _context = context;

        /// <summary>
        /// Lấy dữ liệu doanh thu hôm nay
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTodayRevenue")]
        public MLActionResult GetTodayRevenue(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                decimal todayRevenue = _context.Order.Where(o => o.OrderDate >= minDate && o.Status == ML.Common.EnumOrderStatus.Paid).Sum(o => o.TotalAmount);

                DateTime yesterday = minDate.AddDays(-1);
                decimal yesterdayRevenue = _context.Order.Where(o => o.OrderDate >= yesterday && o.OrderDate < minDate && o.Status == ML.Common.EnumOrderStatus.Paid).Sum(o => o.TotalAmount);

                decimal? revenueTrend = yesterdayRevenue > 0 ? (todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100 : null;

                object dataResult = new
                {
                    Revenue = todayRevenue,
                    Trend = revenueTrend
                };

                result.Data = dataResult;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
