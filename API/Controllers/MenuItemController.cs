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
        public MLActionResult GetMenuItemPaging(int page, int itemsPerPage, string? search)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                IEnumerable<MenuItem> allData = _entities.ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    string normalizedSearchTerm = search.RemoveDiacritics().ToLower();

                    allData = allData.Where(e => e.Name.RemoveDiacritics().ToLower().Contains(normalizedSearchTerm));
                }

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

        /// <summary>
        /// Lấy dữ liệu theo phân trang
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
