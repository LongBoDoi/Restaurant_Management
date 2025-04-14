using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace API.Controllers
{
    public class ReservationController(ApplicationDBContext context) : MLBaseController<Reservation>(context)
    {
        /// <summary>
        /// Tạo đặt bàn mới
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost("CreateCustomerReservation")]
        public MLActionResult CreateCustomerReservation(Reservation reservation)
        {
            MLActionResult result = new();

            try
            {
                reservation.Status = EnumReservationStatus.Pending;
                _context.Reservation.Add(reservation);

                if (_context.SaveChanges() > 0)
                {
                    result.Success = true;
                    result.Data = reservation;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu chi tiết đặt bàn
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public override MLActionResult GetByID(string ID)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                if (Guid.TryParse(ID, out Guid gReservationID))
                {
                    Reservation? reservation = _context.Reservation.AsNoTracking()
                        .Include(r => r.ReservationTables).ThenInclude(rt => rt.Table)
                        .FirstOrDefault(o => o.ReservationID.Equals(gReservationID));
                    if (reservation != null)
                    {
                        reservation.RemoveCircularReferences();

                        result.Data = reservation;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        protected override bool BeforeSave(Reservation reservation, MLActionResult result)
        {
            reservation.ReservationTables.RemoveAllReferences();

            switch (reservation.EditMode)
            {
                case EnumEditMode.Add:
                    // Nếu là thêm đặt bàn mới thì gán tất cả bàn sang trạng thái "Đặt bàn"
                    if (reservation.ReservationTables != null && reservation.ReservationTables.Any())
                    {
                        foreach (ReservationTable reservationTable in reservation.ReservationTables)
                        {
                            Table saveTable = new Table { TableID = reservationTable.TableID, Status = EnumTableStatus.Reserved };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }
                    break;
                case EnumEditMode.Edit:
                    // Nếu là huỷ đặt bàn thì trả tất cả bàn về trạng thái Available
                    if (reservation.Status == EnumReservationStatus.Cancelled)
                    {
                        foreach (ReservationTable reservationTable in reservation.ReservationTables)
                        {
                            Table saveTable = new Table { TableID = reservationTable.TableID, Status = EnumTableStatus.Available };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                        return true;
                    }

                    if (reservation.Status != EnumReservationStatus.Confirmed)
                    {
                        return true;
                    }

                    // Nếu là sửa lại thông tin đặt bàn, nếu bàn nào mới thì gán trạng thái "Đặt bàn", nếu bàn nào bỏ thì gán trạng thái "Còn trống"
                    IEnumerable<ReservationTable> dbReservationTables = _context.ReservationTable.Where(rt => rt.ReservationID.Equals(reservation.ReservationID)).Include(rt => rt.Table);

                    var incomings = reservation.ReservationTables?.Select(ab => ab.TableID).ToHashSet() ?? [];
                    var existings = dbReservationTables.Select(ab => ab.TableID).ToHashSet();

                    // Những bản ghi bị xoá, bàn sẽ được trả về trạng thái Available
                    var toRemove = dbReservationTables.Where(rt => !incomings.Contains(rt.TableID)).ToList();
                    if (toRemove.Any())
                    {
                        _context.ReservationTable.RemoveRange(toRemove);
                        foreach (ReservationTable reservationTable in toRemove)
                        {
                            Table saveTable = new Table { TableID = reservationTable.TableID, Status = EnumTableStatus.Available };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }

                    // Những bản ghi được thêm mới, bàn bị gán là đã đặt chỗ
                    var toAdd = reservation.ReservationTables?.Where(rt => !existings.Contains(rt.TableID)).ToList() ?? [];
                    if (toAdd.Any())
                    {
                        _context.ReservationTable.AddRange(toAdd);
                        foreach (ReservationTable reservationTable in toAdd)
                        {
                            Table saveTable = new Table { TableID = reservationTable.TableID, Status = EnumTableStatus.Reserved };
                            _context.Attach(saveTable);
                            _context.Entry(saveTable).Property(t => t.Status).IsModified = true;
                        }
                    }
                    break;
                case EnumEditMode.Delete:
                    // Nếu là huỷ đặt bàn thì trả tất cả bàn về trạng thái Available
                    if (reservation.ReservationTables != null && reservation.ReservationTables.Any())
                    {
                        foreach (ReservationTable reservationTable in reservation.ReservationTables)
                        {
                            Table saveTable = new Table { TableID = reservationTable.TableID, Status = EnumTableStatus.Available };
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
