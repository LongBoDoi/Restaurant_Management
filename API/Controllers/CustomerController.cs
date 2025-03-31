using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;

namespace API.Controllers
{
    public class CustomerController(ApplicationDBContext context) : MLBaseController<Customer>(context)
    {
        protected override bool BeforeSave(Customer customer, MLActionResult result)
        {
            Customer? existedCustomer = _context.Customer.FirstOrDefault(c =>
                (customer.EditMode == EnumEditMode.Add && ((!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber == customer.PhoneNumber) || (!string.IsNullOrEmpty(c.Email) && c.Email == customer.Email)))
            );
            if (existedCustomer != null)
            {
                result.Success = false;
                result.ErrorMsg = "Số điện thoại hoặc email đã được sử dụng";
                return false;
            }

            return true;
        }
    }
}
