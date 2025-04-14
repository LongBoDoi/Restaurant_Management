using API.ML.BO;
using API.ML.BOBase;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class TableController(ApplicationDBContext context) : MLBaseController<Table>(context)
    {
        protected override bool BeforeSave(Table table, MLActionResult result)
        {
            if (table.EditMode == ML.Common.EnumEditMode.Add && table.Area != null)
            {
                _context.Entry(table.Area).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }

            return true;
        }
    }
}
