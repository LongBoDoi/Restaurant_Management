using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using API.ML.Extensions;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                decimal todayRevenue = _context.Order.Where(o => o.OrderDate >= minDate && o.Status == EnumOrderStatus.Paid).Sum(o => o.TotalAmount);

                DateTime yesterday = minDate.AddDays(-1);
                decimal yesterdayRevenue = _context.Order.Where(o => o.OrderDate >= yesterday && o.OrderDate < minDate && o.Status == EnumOrderStatus.Paid).Sum(o => o.TotalAmount);

                decimal? revenueTrend = yesterdayRevenue > 0 ? Math.Round((todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100) : null;

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

        /// <summary>
        /// Lấy dữ liệu số order đã hoàn thành
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetCompletedOrders")]
        public MLActionResult GetCompletedOrders(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                decimal todayCompletedOrders = _context.Order.Where(o => o.OrderDate >= minDate && o.Status == ML.Common.EnumOrderStatus.Paid).Count();

                DateTime yesterday = minDate.AddDays(-1);
                decimal yesterdayCompletedOrders = _context.Order.Where(o => o.OrderDate >= yesterday && o.OrderDate < minDate && o.Status == ML.Common.EnumOrderStatus.Paid).Count();

                decimal? trend = yesterdayCompletedOrders > 0 ? Math.Round((todayCompletedOrders - yesterdayCompletedOrders) / yesterdayCompletedOrders * 100) : null;

                object dataResult = new
                {
                    CompletedOrders = todayCompletedOrders,
                    Trend = trend
                };

                result.Data = dataResult;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Giá trị order trung bình
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetAverageOrderValue")]
        public MLActionResult GetAverageOrderValue(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var todayOrders = _context.Order.Where(o => o.OrderDate >= minDate && o.Status == ML.Common.EnumOrderStatus.Paid).ToList();
                decimal todayAverage = todayOrders.Any() ? todayOrders.Average(o => o.NetAmount) : 0;

                DateTime yesterday = minDate.AddDays(-1);
                var yesterdayOrders = _context.Order.Where(o => o.OrderDate >= yesterday && o.OrderDate < minDate && o.Status == ML.Common.EnumOrderStatus.Paid).ToList();
                decimal yesterdayAverage = yesterdayOrders.Any() ? yesterdayOrders.Average(o => o.NetAmount) : 0;

                decimal? trend = yesterdayAverage > 0 ? Math.Round((todayAverage - yesterdayAverage) / yesterdayAverage * 100) : null;

                object dataResult = new
                {
                    AverageOrderValue = Math.Round(todayAverage),
                    Trend = trend
                };

                result.Data = dataResult;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Giá trị order trung bình
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetActiveTables")]
        public MLActionResult GetActiveTables()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var allTables = _context.Table.ToList();
                var occupiedTables = allTables.Where(t => t.Status == EnumTableStatus.Occupied);

                decimal allTablesCount = allTables.Count();
                decimal occupiedTablesCount = occupiedTables.Count();
                decimal? occupancyRate = allTablesCount > 0 ? Math.Round(occupiedTablesCount / allTablesCount * 100) : null;

                object dataResult = new
                {
                    AllTablesCount = allTablesCount,
                    OccupiedTablesCount = occupiedTablesCount,
                    OccupancyRate = occupancyRate
                };

                result.Data = dataResult;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Xu hướng doanh thu
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetRevenueTrend")]
        public MLActionResult GetRevenueTrend(DateTime today, int timeFilter)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                List<object> data = [];

                var fromDate = today;
                switch (timeFilter)
                {
                    // Lọc theo ngày
                    case 0:
                        fromDate = today.AddDays(-30);
                        break;
                    // Lọc theo tuần
                    case 1:
                        fromDate = today.AddDays(-70);
                        break;
                    // Lọc theo tháng
                    case 2:
                        fromDate = today.AddMonths(-12);
                        break;
                }

                while (fromDate <= today)
                {
                    var toDate = fromDate;
                    switch (timeFilter)
                    {
                        case 0:
                            toDate = fromDate.AddDays(1);
                            break;
                        case 1:
                            toDate = fromDate.AddDays(7 - (int)fromDate.DayOfWeek);
                            break;
                        case 2:
                            toDate = fromDate.AddMonths(1);
                            break;

                    }
                    decimal revenue = _context.Order.Where(o => o.OrderDate >= fromDate && o.OrderDate < toDate && o.Status == EnumOrderStatus.Paid).Sum(o => o.TotalAmount);

                    data.Add(new
                    {
                        FromDate = fromDate,
                        ToDate = toDate,
                        Revenue = revenue
                    });

                    fromDate = toDate;
                }

                result.Data = data;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Món ăn phổ biến
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetPopularMenuItems")]
        public MLActionResult GetPopularMenuItems(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                DateTime yesterday = minDate.AddDays(-1);

                var data = _context.MenuItem
                    .Select(mi => new
                    {
                        MenuItem = mi,
                        TodayCount = mi.OrderDetails
                            .Count(od => od.Order != null && od.Order.OrderDate >= minDate && od.Order.Status == EnumOrderStatus.Paid),
                        YesterdayCount = mi.OrderDetails
                            .Count(od => od.Order != null && od.Order.OrderDate >= yesterday && od.Order.OrderDate < minDate && od.Order.Status == EnumOrderStatus.Paid)
                    })
                    .Where(mi => mi.TodayCount > 0)
                    .OrderByDescending(x => x.TodayCount)
                    .ThenBy(x => x.MenuItem.Name)
                    .ToList()
                    .Select(d =>
                        new {
                            MenuItem = d.MenuItem,
                            Count = d.TodayCount,
                            Trend = (decimal?)(d.YesterdayCount > 0 ? Math.Round((decimal)(d.TodayCount - d.YesterdayCount) / (decimal)d.YesterdayCount * 100) : null)
                    })
                    .ToList();

                result.Data = data;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Order gần đây
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetRecentOrders")]
        public MLActionResult GetRecentOrders(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var data = _context.Order
                    .Include(o => o.OrderDetails)
                    .Where(o => o.OrderDate >= minDate)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                data.RemoveCircularReferences();

                result.Data = data;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu Món ăn phổ biến
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTodayReservations")]
        public MLActionResult GetTodayReservations(DateTime minDate)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var toDate = minDate.AddDays(1);

                var data = _context.Reservation
                    .Include(r => r.Customer)
                    .Where(r => r.ReservationDate >= minDate && r.ReservationDate < toDate)
                    .OrderBy(r => r.ReservationDate)
                    .ToList();

                data.RemoveCircularReferences();

                result.Data = data;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
