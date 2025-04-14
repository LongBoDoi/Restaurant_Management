using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class AreaController(ApplicationDBContext context) : MLBaseController<Area>(context)
    {
    }
}
