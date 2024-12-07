using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class OrderController(ApplicationDBContext context) : MLBaseController<Order>(context)
    {
        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetOrdersByStatus")]
        public MLActionResult GetOrdersByStatus(EnumOrderStatus status, int page, int itemsPerPage)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var allOrders = _entities.AsNoTracking().Where(o => o.Status == status);
                if (itemsPerPage != -1)
                {
                    allOrders = allOrders.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
                }
                result.Data = new
                {
                    Data = allOrders.ToList().Select(o =>
                    {
                        o.OrderDetails = _context.OrderDetail.AsNoTracking().Where(od => od.OrderID == o.OrderID).ToList().Select(od =>
                        {
                            od.MenuItem = _context.MenuItem.AsNoTracking().FirstOrDefault(mi => mi.MenuItemID == od.MenuItemID);
                            return od;
                        });
                        o.Customer = _context.Customer.AsNoTracking().FirstOrDefault(c => c.CustomerID == o.CustomerID);
                        return o;
                    }),
                    TotalCount = _entities.Count()
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        protected override bool BeforeSave(Order order, MLActionResult result)
        {
            order.Customer = null;
            if (order.EditMode == ML.Common.EnumEditMode.Add)
            {
                order.OrderDate = DateTime.UtcNow;
                foreach (var orderDetail in order.OrderDetails)
                {
                    orderDetail.MenuItem = null;
                    orderDetail.Order = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Xử lý sau khi save thành công
        /// </summary>
        protected override void AfterSaveSuccess(Order order)
        {
            foreach (var orderDetail in order.OrderDetails)
            {
                orderDetail.MenuItem = null;
                orderDetail.Order = null;
            }
        }
    }
}
