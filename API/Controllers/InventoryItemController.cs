using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class InventoryItemController(ApplicationDBContext context) : MLBaseController<InventoryItem>(context)
    {
    }
}
