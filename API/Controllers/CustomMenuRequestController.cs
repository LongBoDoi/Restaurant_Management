using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class CustomMenuRequestController(ApplicationDBContext context) : MLBaseController<CustomMenuRequest>(context)
    {
        protected override bool BeforeSave(CustomMenuRequest entity, MLActionResult result)
        {
            if (entity.SaveToOrder && entity.OrderID.HasValue)
            {
                Order existOrder = new Order { OrderID = (Guid)entity.OrderID };
                _context.Attach(existOrder);

                existOrder.OrderDetails.Add(new OrderDetail
                {
                    MenuItemName = entity.MenuItemName,
                    Price = entity.Price,
                    Quantity = 1,
                    Amount = entity.Price * 1,
                    Note = entity.Note
                });
            }

            if (entity.SaveToMenu)
            {
                _context.MenuItem.Add(new MenuItem
                {
                    Name = entity.MenuItemName,
                    Price = entity.Price
                });
            }

            if (entity.EditMode == EnumEditMode.Edit)
            {
                entity.InventoryItems = null;
            }
            _context.Attach(entity);

            return true;
        }
    }
}
