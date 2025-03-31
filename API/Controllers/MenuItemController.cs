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
    public class MenuItemController : MLBaseController<MenuItem>
    {
        public MenuItemController(ApplicationDBContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetMenuItemPaging")]
        public MLActionResult GetMenuItemPaging(int page, int itemsPerPage, string? search, string? categoryID)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                IEnumerable<MenuItem> allData = _entities.AsNoTracking().ToList();

                string? normalizedSearchTerm = search?.RemoveDiacritics().ToLower();
                Guid.TryParse(categoryID, out Guid gCategoryID);

                allData = allData.Where(e => (string.IsNullOrEmpty(normalizedSearchTerm) || e.Name.RemoveDiacritics().ToLower().Contains(normalizedSearchTerm))
                    && (gCategoryID == Guid.Empty || e.MenuItemCategoryID == gCategoryID)
                ).Select(mi =>
                {
                    mi.MenuItemCategory = _context.MenuItemCategory.FirstOrDefault(mic => mic.MenuItemCategoryID == mi.MenuItemCategoryID);
                    return mi;
                });

                int totalCount = allData.Count();

                result.Data = new
                {
                    Data = allData.OrderByDescending(e => e.CreatedDate).Skip((page - 1) * itemsPerPage).Take(itemsPerPage),
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lưu món
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateMenuItem")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public MLActionResult UpdateMenuItem([FromForm] string menuItem, [FromForm] IFormFile? image)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                MenuItem? objMenuItem = JsonConvert.DeserializeObject<MenuItem>(menuItem);

                if (objMenuItem != null)
                {
                    if (objMenuItem.EditMode != EnumEditMode.Delete && image != null)
                    {
                        string imageUrl = CommonFunction.SaveImage(image);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            objMenuItem.ImageUrl = imageUrl;
                        }
                    }

                    objMenuItem.MenuItemCategory = null;
                    result = SaveChanges(objMenuItem);
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
