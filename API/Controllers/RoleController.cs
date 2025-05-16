using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.X509;

namespace API.Controllers
{
    public class RoleController(ApplicationDBContext context) : MLBaseController<Role>(context)
    {
        protected override bool BeforeSave(Role entity, MLActionResult result)
        {
            // Kiểm tra trùng mã vai trò
            string code = entity.RoleCode.Trim().ToLower();
            bool duplicateCode = _context.Role.FirstOrDefault(e => e.RoleCode.Trim().ToLower() == code &&
                (entity.EditMode == EnumEditMode.Add || (entity.EditMode == EnumEditMode.Edit && e.RoleID != entity.RoleID))
            ) != null;
            if (duplicateCode)
            {
                result.Success = false;
                result.ErrorMsg = "Mã vai trò đã tồn tại.";
                return false;
            }

            switch (entity.EditMode)
            {
                case EnumEditMode.Add:
                    if (entity.Employees != null && entity.Employees.Any())
                    {
                        foreach (Employee employee in entity.Employees)
                        {
                            _context.Entry(employee).State = EntityState.Unchanged;
                        }
                    }

                    if (entity.Permissions != null && entity.Permissions.Any())
                    {
                        foreach (Permission permission in entity.Permissions)
                        {
                            _context.Entry(permission).State = EntityState.Unchanged;
                        }
                    }
                    break;
                case EnumEditMode.Edit:
                    var existingRole = _context.Role
                        .Include(r => r.Employees)
                        .Include(r => r.Permissions)
                        .FirstOrDefault(r => r.RoleID == entity.RoleID);

                    if (existingRole == null)
                    {
                        return false;
                    }

                    if (existingRole.Employees != null && entity.Employees != null)
                    {
                        // Clear existing employees
                        ((HashSet<Employee>)existingRole.Employees).Clear();

                        // Add updated employees (make sure they're attached/tracked)
                        foreach (var employee in entity.Employees)
                        {
                            var trackedEmployee = _context.Employee.Find(employee.EmployeeID);
                            if (trackedEmployee != null)
                            {
                                ((HashSet<Employee>)existingRole.Employees).Add(trackedEmployee);
                            }
                        }
                    }

                    if (existingRole.Permissions != null && entity.Permissions != null)
                    {
                        ((HashSet<Permission>)existingRole.Permissions).Clear();

                        foreach (var permission in entity.Permissions)
                        {
                            var trackedPermission = _context.Permission.Find(permission.PermissionID);
                            if (trackedPermission != null)
                            {
                                ((HashSet<Permission>)existingRole.Permissions).Add(trackedPermission);
                            }
                        }
                    }

                    existingRole.AssignValuesFrom(entity);
                    _context.Entry(existingRole).State = EntityState.Modified;

                    entity.EditMode = null;
                    break;
            }

            return base.BeforeSave(entity, result);
        }
    }
}
