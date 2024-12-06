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
                    Data = _entities.AsNoTracking().Where(e => e.Role != EnumRole.Admin).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList().Select(e =>
                    {
                        e.UserLogin = _context.UserLogin.AsNoTracking().FirstOrDefault(ul => ul.EmployeeID == e.EmployeeID);
                        if (e.UserLogin != null)
                        {
                            e.UserLogin.Password = string.Empty;
                        }
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

        protected override bool BeforeSave(Employee employee, MLActionResult result)
        {
            if (employee.EditMode == EnumEditMode.Edit && employee.UserLogin != null && employee.UserLogin.IsChangePassword)
            {
                string? oldPassword = _entities.AsNoTracking().Include(e => e.UserLogin).FirstOrDefault(e => e.EmployeeID == employee.EmployeeID)?.UserLogin?.Password;
                if (!string.IsNullOrEmpty(oldPassword) && employee.UserLogin.OldPassword != oldPassword)
                {
                    result.ErrorMsg = "Mật khẩu cũ không chính xác.";
                    result.Success = false;

                    return false;
                } else
                {
                    _context.Entry(employee.UserLogin).State = EntityState.Modified;
                }
            }
            return true;
        }

        protected override void AfterSaveSuccess(Employee employee)
        {
            if (employee.UserLogin != null)
            {
                employee.UserLogin = null;
            }
        }
    }
}
