using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
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
        [HttpGet("GetOrderDetail")]
        public MLActionResult GetOrderDetail(string orderID)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                if (Guid.TryParse(orderID, out Guid gOrderID))
                {
                    Order? order = _context.Order.AsNoTracking()
                        .Include(o => o.OrderDetails).ThenInclude(od => od.MenuItem)
                        .Include(o => o.Customer)
                        .Include(o => o.OrderTables).ThenInclude(ot => ot.Table)
                        .FirstOrDefault(o => o.OrderID.Equals(gOrderID));
                    if (order != null)
                    {
                        order.RemoveCircularReferences();

                        result.Data = order;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetActiveOrderByCustomerID")]
        public MLActionResult GetActiveOrderByCustomerID(string customerID)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                if (Guid.TryParse(customerID, out Guid gCustomerID))
                {
                    Order? order = _context.Order.AsNoTracking()
                        .FirstOrDefault(o => o.CustomerID.Equals(gCustomerID) && o.Status == EnumOrderStatus.Active);

                    result.Data = order;
                }
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
            order.OrderDetails.RemoveAllReferences();
            order.OrderTables.RemoveAllReferences();

            switch (order.EditMode)
            {
                case EnumEditMode.Add:
                    order.OrderDate = DateTime.UtcNow;

                    // Nếu là tạo order mới mà chưa thanh toán thì gán tất cả bàn sang trạng thái "Hết chỗ"
                    if (order.OrderTables != null && order.OrderTables.Any())
                    {
                        foreach (OrderTable orderTable in order.OrderTables)
                        {
                            Table saveTable = new Table { TableID = orderTable.TableID, Status = EnumTableStatus.Occupied };

                            if (order.Status == EnumOrderStatus.Paid)
                            {
                                saveTable.Status = EnumTableStatus.Available;
                            } else
                            {
                                saveTable.Status = EnumTableStatus.Occupied;
                            }

                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }
                    break;
                case EnumEditMode.Edit:
                    //Nếu là thanh toán hoặc huỷ order thì trả tất cả bàn trong order về trạng thái available
                    if (order.Status == EnumOrderStatus.Paid || order.Status == EnumOrderStatus.Canceled)
                    {
                        if (order.OrderTables != null && order.OrderTables.Any())
                        {
                            foreach (OrderTable orderTable in order.OrderTables)
                            {
                                Table saveTable = new Table { TableID = orderTable.TableID, Status = EnumTableStatus.Available };
                                _context.Attach(saveTable);
                                _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                            }
                        }

                        return true;
                    }

                    // Nếu là đổi bàn cho order, nếu bàn nào mới thì gán trạng thái "Hết chỗ", nếu bàn nào bỏ thì gán trạng thái "Còn trống"
                    IEnumerable<OrderTable> dbOrderTables = _context.OrderTable.Where(rt => rt.OrderID.Equals(order.OrderID)).Include(rt => rt.Table);

                    var incomings = order.OrderTables?.Select(ab => ab.TableID).ToHashSet() ?? [];
                    var existings = dbOrderTables.Select(ab => ab.TableID).ToHashSet();

                    // Những bản ghi bị xoá, bàn sẽ được trả về trạng thái Available
                    var toRemove = dbOrderTables.Where(rt => !incomings.Contains(rt.TableID)).ToList();
                    if (toRemove.Any())
                    {
                        _context.OrderTable.RemoveRange(toRemove);
                        foreach (OrderTable orderTable in toRemove)
                        {
                            Table saveTable = new Table { TableID = orderTable.TableID, Status = EnumTableStatus.Available };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }

                    // Những bản ghi được thêm mới, bàn bị gán là hết chỗ
                    var toAdd = order.OrderTables?.Where(rt => !existings.Contains(rt.TableID)).ToList() ?? [];
                    if (toAdd.Any())
                    {
                        _context.OrderTable.AddRange(toAdd);
                        foreach (OrderTable orderTable in toAdd)
                        {
                            Table saveTable = new Table { TableID = orderTable.TableID, Status = EnumTableStatus.Occupied };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }
                    break;
            }

            return true;
        }
    }
}
