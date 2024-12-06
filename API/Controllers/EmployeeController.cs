using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class EmployeeController(ApplicationDBContext context) : MLBaseController<Employee>(context)
    {
        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetDataPaging")]
        public override MLActionResult GetDataPaging(int page, int itemsPerPage)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = new
                {
                    Data = _entities.Where(e => e.Role != EnumRole.Admin).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList().Select(e =>
                    {
                        e.UserLogin = _context.UserLogin.AsNoTracking().FirstOrDefault(ul => ul.EmployeeID == e.EmployeeID);
                        return e;
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

        public override void AfterSaveSuccess(Employee employee)
        {
            if (employee.UserLogin != null)
            {
                employee.UserLogin.Employee = null;
            }
        }
    }
}
