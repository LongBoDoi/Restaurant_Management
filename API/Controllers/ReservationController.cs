using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ReservationController : MLBaseController<MenuItem>
    {
        public ReservationController(ApplicationDBContext context) : base(context)
        {
        }

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
                result.Success = _context.SaveChanges() == 1;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách đặt bàn
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetReservations")]
        public MLActionResult GetReservations(EnumReservationStatus status, int page, int itemsPerPage)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = new
                {
                    Reservations = _context.Reservation.Where(r => r.Status == status).OrderBy(r => r.ReservationDate).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList(),
                    TotalCount = _context.Reservation.Count()
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật bản ghi reservation
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateReservationInfo")]
        public MLActionResult UpdateReservationInfo(Reservation reservation)
        {
            MLActionResult result = new();

            try
            {
                switch (reservation.EditMode)
                {
                    case EnumEditMode.Add:
                        _context.Reservation.Add(reservation);
                        break;
                    case EnumEditMode.Edit:
                        _context.Entry(reservation).State = EntityState.Modified;
                        break;
                    case EnumEditMode.Delete:
                        _context.Entry(reservation).State = EntityState.Deleted;
                        break;
                }

                result.Success = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
