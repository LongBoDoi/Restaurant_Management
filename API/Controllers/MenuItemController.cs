using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MenuItemController : MLBaseController<MenuItem>
    {
        public MenuItemController(ApplicationDBContext context) : base(context)
        {
        }

        /// <summary>
        /// Tạo món mới
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateMenuItemInfo")]
        public MLActionResult UpdateMenuItemInfo(MenuItem menuItem)
        {
            MLActionResult result = new();

            try
            {
                switch (menuItem.EditMode)
                {
                    case EnumEditMode.Add:
                        MenuItem? existMenuItem = _context.MenuItem.FirstOrDefault(mi => mi.Name.Equals(menuItem.Name, StringComparison.OrdinalIgnoreCase));
                        if (existMenuItem != null)
                        {
                            result.Success = false;
                            result.ErrorMsg = "Tên món đã tồn tại.";
                            return result;
                        }

                        menuItem.MenuItemID = Guid.NewGuid();
                        _context.MenuItem.Add(menuItem);

                        break;
                    case EnumEditMode.Edit:
                        _context.Entry(menuItem).State = EntityState.Modified;
                        break;
                    case EnumEditMode.Delete:
                        _context.Entry(menuItem).State = EntityState.Deleted;
                        break;
                }


                result.Success = _context.SaveChanges() > 0;
                if (menuItem.EditMode != EnumEditMode.Delete)
                {
                    result.Data = menuItem;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Tạo món mới
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetMenuItems")]
        public MLActionResult GetMenuItems(int page, int itemsPerPage)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = new
                {
                    MenuItems = _context.MenuItem.OrderBy(mi => mi.Name).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList(),
                    TotalCount = _context.MenuItem.Count()
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
