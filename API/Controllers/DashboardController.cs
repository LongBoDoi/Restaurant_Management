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
        [HttpGet("GetTotalRevenue")]
        public MLActionResult GetTotalRevenue(DateTime fromDate, DateTime toDate, EnumTimeFilter timeFilter, int timeZone)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                decimal currentRevenue = _context.Order
                    .Where(o => o.OrderDate >= fromDate.ApplyTimeZone(timeZone) && o.OrderDate < toDate.ApplyTimeZone(timeZone) && o.Status == EnumOrderStatus.Paid)
                    .Sum(o => o.TotalAmount);

                DateTime previousFromDate = fromDate.GetFromDateByTimeFilter(timeFilter);
                decimal previousRevenue = _context.Order
                    .Where(o => o.OrderDate >= previousFromDate.ApplyTimeZone(timeZone) && o.OrderDate < fromDate.ApplyTimeZone(timeZone) && o.Status == EnumOrderStatus.Paid)
                    .Sum(o => o.TotalAmount);

                decimal? revenueTrend = previousRevenue > 0 ? Math.Round((currentRevenue - previousRevenue) / previousRevenue * 100) : null;

                object dataResult = new
                {
                    Revenue = currentRevenue,
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
        /// Lấy dữ liệu tổng số order
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetTotalOrders")]
        public MLActionResult GetTotalOrders(DateTime fromDate, DateTime toDate, EnumTimeFilter timeFilter, int timeZone)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                decimal currentTotalOrders = _context.Order
                    .Where(o => o.OrderDate >= fromDate.ApplyTimeZone(timeZone) && o.OrderDate < toDate.ApplyTimeZone(timeZone))
                    .Count();

                DateTime previousFromDate = fromDate.GetFromDateByTimeFilter(timeFilter);
                decimal previousTotalOrders = _context.Order
                    .Where(o => o.OrderDate >= previousFromDate.ApplyTimeZone(timeZone) && o.OrderDate < fromDate.ApplyTimeZone(timeZone))
                    .Count();

                decimal? trend = previousTotalOrders > 0 ? Math.Round((currentTotalOrders - previousTotalOrders) / previousTotalOrders * 100) : null;

                object dataResult = new
                {
                    CompletedOrders = currentTotalOrders,
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
        public MLActionResult GetAverageOrderValue(DateTime fromDate, DateTime toDate, EnumTimeFilter timeFilter, int timeZone)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var currentOrders = _context.Order
                    .Where(o => o.OrderDate >= fromDate.ApplyTimeZone(timeZone) && o.OrderDate < toDate.ApplyTimeZone(timeZone) && o.Status == EnumOrderStatus.Paid)
                    .ToList();
                decimal currentAverage = currentOrders.Any() ? currentOrders.Average(o => o.NetAmount) : 0;

                DateTime previousFromDate = fromDate.GetFromDateByTimeFilter(timeFilter);
                var previousOrders = _context.Order
                    .Where(o => o.OrderDate >= previousFromDate.ApplyTimeZone(timeZone) && o.OrderDate < fromDate.ApplyTimeZone(timeZone) && o.Status == EnumOrderStatus.Paid)
                    .ToList();
                decimal previousAverage = previousOrders.Any() ? previousOrders.Average(o => o.NetAmount) : 0;

                decimal? trend = previousAverage > 0 ? Math.Round((currentAverage - previousAverage) / previousAverage * 100) : null;

                object dataResult = new
                {
                    AverageOrderValue = Math.Round(currentAverage),
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
        public MLActionResult GetRevenueTrend(DateTime fromDate, DateTime toDate, EnumTimeFilter timeFilter, int timeZone)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                List<object> data = [];

                while (fromDate < toDate)
                {
                    DateTime nextDate = fromDate.GetToDateByTimeFilter(timeFilter);
                    if (nextDate > toDate)
                    {
                        nextDate = toDate;
                    }
                    decimal revenue = _context.Order
                        .Where(o => o.OrderDate >= fromDate.ApplyTimeZone(timeZone) && o.OrderDate < nextDate.ApplyTimeZone(timeZone) && o.Status == EnumOrderStatus.Paid)
                        .Sum(o => o.TotalAmount);

                    data.Add(new
                    {
                        FromDate = fromDate,
                        ToDate = nextDate,
                        Revenue = revenue
                    });

                    fromDate = nextDate;
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
        public MLActionResult GetPopularMenuItems(DateTime fromDate, DateTime toDate, EnumTimeFilter timeFilter, int timeZone)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                DateTime previousFromDate = fromDate.GetFromDateByTimeFilter(timeFilter);

                var data = _context.MenuItem
                    .Select(mi => new
                    {
                        MenuItem = mi,
                        CurrentCount = mi.OrderDetails
                            .Count(od => 
                                od.Order != null && 
                                od.Order.OrderDate >= fromDate.ApplyTimeZone(timeZone) && 
                                od.Order.OrderDate < toDate.ApplyTimeZone(timeZone) && 
                                od.Order.Status == EnumOrderStatus.Paid
                            ),
                        PreviousCount = mi.OrderDetails
                            .Count(od => 
                                od.Order != null && 
                                od.Order.OrderDate >= previousFromDate.ApplyTimeZone(timeZone) && 
                                od.Order.OrderDate < fromDate.ApplyTimeZone(timeZone) && 
                                od.Order.Status == EnumOrderStatus.Paid
                            )
                    })
                    .Where(mi => mi.CurrentCount > 0)
                    .OrderByDescending(x => x.CurrentCount)
                    .ThenBy(x => x.MenuItem.Name)
                    .ToList()
                    .Select(d =>
                        new {
                            MenuItem = d.MenuItem,
                            Count = d.CurrentCount,
                            Trend = (decimal?)(d.PreviousCount > 0 ? Math.Round((decimal)(d.CurrentCount - d.PreviousCount) / (decimal)d.PreviousCount * 100) : null)
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
