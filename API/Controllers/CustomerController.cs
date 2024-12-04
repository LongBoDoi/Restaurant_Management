using API.ML.BO;
using API.ML.BOBase;

namespace API.Controllers
{
    public class CustomerController(ApplicationDBContext context) : MLBaseController<Customer>(context)
    {
    }
}
