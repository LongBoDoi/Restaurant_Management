using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class CustomerController(ApplicationDBContext context) : MLBaseController<Customer>(context)
    {
        /// <summary>
        /// Lưu món
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdatePersonalInfo")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public MLActionResult UpdatePersonalInfo([FromForm] string customerJson, [FromForm] IFormFile? image)
        {
            MLActionResult result = new();

            try
            {
                Customer? customer = JsonConvert.DeserializeObject<Customer>(customerJson);

                if (customer != null)
                {
                    if (customer.EditMode != EnumEditMode.Delete && image != null)
                    {
                        string imageUrl = CommonFunction.SaveImage(image);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            customer.ImageUrl = imageUrl;
                        }
                    }

                    result = SaveChanges(customer);
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

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
