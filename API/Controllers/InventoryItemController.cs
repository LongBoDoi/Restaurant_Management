using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class InventoryItemController(ApplicationDBContext context) : MLBaseController<InventoryItem>(context)
    {
        protected override bool BeforeSave(InventoryItem entity, MLActionResult result)
        {
            if (entity.InventoryItemCategory != null)
            {
                _context.Entry(entity.InventoryItemCategory).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }

            return true;
        }
    }
}
