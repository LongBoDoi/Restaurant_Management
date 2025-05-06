using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class PermissionController(ApplicationDBContext context) : MLBaseController<Permission>(context)
    {
    }
}
