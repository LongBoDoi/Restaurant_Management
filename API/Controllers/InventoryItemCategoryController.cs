using API.ML.BO;
using API.ML.BOBase;
using API.ML.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class InventoryItemCategoryController(ApplicationDBContext context) : MLBaseController<InventoryItemCategory>(context)
    {
        protected override void PrepareExtraData(IEnumerable<InventoryItemCategory> entites)
        {
            foreach (InventoryItemCategory category in entites)
            {
                category.InventoryItemCount = _context.InventoryItem.Where(x => x.InventoryItemCategoryID == category.InventoryItemCategoryID).Count();
            }
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [HttpGet("GetInventoryItemForCustomerCreate")]
        public virtual MLActionResult GetInventoryItemForCustomerCreate()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                IEnumerable<InventoryItemCategory> categories = _context.InventoryItemCategory.AsNoTracking().Where(x => !x.Inactive).ToList();
                foreach (InventoryItemCategory category in categories)
                {
                    category.InventoryItems = _context.InventoryItem.AsNoTracking().Where(x => x.InventoryItemCategoryID == category.InventoryItemCategoryID && !x.Inactive && x.Quantity > 0).ToList();
                }

                result.Data = categories;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
