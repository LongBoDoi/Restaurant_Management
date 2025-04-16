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
        public override MLActionResult GetAll()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = _entities.OrderBy(e => e.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy phân trang nhóm thực đơn
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Authorize]
        public override MLActionResult GetDataPaging(int page, int itemsPerPage, string? search, string? filter)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                IEnumerable<MenuItemCategory> allData = _entities.ApplyFilters(filter).ToList();

                string? normalizedSearchTerm = search?.RemoveDiacritics().ToLower();

                allData = allData.Where(e => (string.IsNullOrEmpty(normalizedSearchTerm) || e.MenuItemCategoryName.RemoveDiacritics().ToLower().Contains(normalizedSearchTerm)))
                    .OrderBy(e => e.SortOrder).Select(mic =>
                    {
                        mic.ItemCount = _context.MenuItem.Where(mi => mi.MenuItemCategoryID == mic.MenuItemCategoryID).Count();
                        return mic;
                    }
                );

                int totalCount = allData.Count();

                result.Data = new
                {
                    Data = allData.Skip((page - 1) * itemsPerPage).Take(itemsPerPage),
                    TotalCount = totalCount
                };
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
