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
    public class MenuItemCategoryController : MLBaseController<MenuItemCategory>
    {
        public MenuItemCategoryController(ApplicationDBContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        public override MLActionResult GetAll(string? filter)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = _entities.ApplyFilters(filter).OrderBy(e => e.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        protected override bool BeforeSave(MenuItemCategory entity, MLActionResult result)
        {
            if (entity.EditMode == EnumEditMode.Add)
            {
                int itemCount = _context.MenuItemCategory.Count();
                entity.SortOrder = itemCount > 0 ? _context.MenuItemCategory.Max(mic => mic.SortOrder) + 1 : 0;
            }

            return true;
        }
    }
}
