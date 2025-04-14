using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class EmployeeController(ApplicationDBContext context) : MLBaseController<Employee>(context)
    {
        /// <summary>
        /// Lưu món
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateEmployee")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public MLActionResult UpdateEmployee([FromForm] string employee, [FromForm] IFormFile? image)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                Employee? objEmployee = JsonConvert.DeserializeObject<Employee>(employee);

                if (objEmployee != null)
                {
                    if (objEmployee.EditMode != EnumEditMode.Delete && image != null)
                    {
                        string imageUrl = CommonFunction.SaveImage(image);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            objEmployee.ImageUrl = imageUrl;
                        }
                    }

                    result = SaveChanges(objEmployee);
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        protected override bool BeforeSave(Employee employee, MLActionResult result)
        {
            // Kiểm tra mật khẩu cũ khi đổi mật khẩu
            if (employee.EditMode == EnumEditMode.Edit && employee.UserLogin != null && employee.UserLogin.IsChangePassword)
            {
                string? oldPassword = _entities.AsNoTracking().Include(e => e.UserLogin).FirstOrDefault(e => e.EmployeeID == employee.EmployeeID)?.UserLogin?.Password;
                if (!string.IsNullOrEmpty(oldPassword) && employee.UserLogin.OldPassword != oldPassword)
                {
                    result.ErrorMsg = "Mật khẩu cũ không chính xác.";
                    result.Success = false;

                    return false;
                }
                else
                {
                    _context.Entry(employee.UserLogin).State = EntityState.Modified;
                }
            }
            return true;
        }
    }
}
