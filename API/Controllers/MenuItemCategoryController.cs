using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class MenuItemCategoryController : MLBaseController<MenuItemCategory>
    {
        public MenuItemCategoryController(ApplicationDBContext context) : base(context)
        {
        }
    }
}
