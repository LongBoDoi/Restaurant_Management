using AngleSharp.Dom;
using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
using API.ML.Extensions;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

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
            MLActionResult result = new();

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
            // Kiểm tra trùng mã NV
            string employeeCode = employee.EmployeeCode.Trim().ToLower();
            Employee? duplicateEmployeeCode = _context.Employee.FirstOrDefault(e => e.EmployeeCode.Trim().ToLower() == employeeCode &&
                (employee.EditMode == EnumEditMode.Add || (employee.EditMode == EnumEditMode.Edit && e.EmployeeID != employee.EmployeeID))
            );
            if (duplicateEmployeeCode != null)
            {
                result.Success = false;
                result.ErrorMsg = "Mã nhân viên đã tồn tại.";
                return false;
            }

            switch (employee.EditMode)
            {
                case EnumEditMode.Add:
                    if (employee.Roles != null && employee.Roles.Any())
                    {
                        foreach (Role role in employee.Roles)
                        {
                            _context.Entry(role).State = EntityState.Unchanged;
                        }
                    }

                    if (employee.UserLogin != null)
                    {
                        employee.UserLogin.Password = PasswordHasherService.HashPassword(employee.UserLogin.Password);
                    }
                    break;
                case EnumEditMode.Edit:
                    var existingEmployee = _context.Employee
                        .Include(e => e.Roles)
                        .FirstOrDefault(e => e.EmployeeID == employee.EmployeeID);

                    if (existingEmployee == null)
                    {
                        return false;
                    }

                    if (existingEmployee.Roles != null && employee.Roles != null)
                    {
                        ((HashSet<Role>)existingEmployee.Roles).Clear();

                        foreach (var role in employee.Roles)
                        {
                            var trackedRole = _context.Role.Find(role.RoleID);
                            if (trackedRole != null)
                            {
                                ((HashSet<Role>)existingEmployee.Roles).Add(trackedRole);
                            }
                        }
                    }

                    existingEmployee.AssignValuesFrom(employee);
                    _context.Entry(existingEmployee).State = EntityState.Modified;

                    employee.EditMode = null;
                    break;
            }

            return true;
        }
    }
}
