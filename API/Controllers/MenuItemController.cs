using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class MenuItemController : MLBaseController<MenuItem>
    {
        public MenuItemController(ApplicationDBContext context) : base(context)
        {
        }
    }
}
