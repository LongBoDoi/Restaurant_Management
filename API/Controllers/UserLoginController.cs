using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : MLBaseController<MenuItem>
    {
        private class SessionInterface
        {
            public string UserName { get; set; } = string.Empty;

            public object? UserData { get; set; }

            public EnumUserType? UserType { get; set; }
        }

        public UserLoginController(ApplicationDBContext context) : base(context)
        {
        }

        /// <summary>
        /// Tạo tài khoản khách hàng mới
        /// </summary>
        /// <param name="customer">Dữ liệu khách hàng</param>
        /// <returns></returns>
        [HttpPost("RegisterNewCustomer")]
        public MLActionResult RegisterNewCustomer(Customer customer)
        {
            MLActionResult result = new();

            try
            {
                if (customer.UserLogin != null)
                {
                    UserLogin? existUser = _context.UserLogin.FirstOrDefault(ul => ul.Username.Equals(customer.UserLogin.Username, StringComparison.OrdinalIgnoreCase));
                    if (existUser != null)
                    {
                        result.Success = false;
                        result.ErrorCode = EnumApplicationErrorCode.DuplicateLoginInfo;
                        return result;
                    }

                    _context.Customer.Add(customer);
                    result.Success = _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Tạo tài khoản admin để đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpPost("CreateAdminUser")]
        public MLActionResult CreateAdminUser()
        {
            MLActionResult result = new();

            try
            {
                Employee employee = new()
                {
                    EmployeeCode = "admin",
                    EmployeeName = "Admin",
                    Role = EnumRole.Admin,
                    UserLogin = new()
                    {
                        Username = "admin",
                        Password = "123456"
                    }
                };
                _context.Employee.Add(employee);
                result.Success = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Thực hiện đăng nhập nhân viên
        /// </summary>
        /// <param name="users">Dữ liệu tài khoản</param>
        /// <returns></returns>
        [HttpPost("LoginEmployee")]
        public MLActionResult LoginEmployee(UserLogin userLogin)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                Employee? employee = _context.Employee.Include(x => x.UserLogin).FirstOrDefault(x => x.UserLogin != null && 
                                                                                       userLogin.Username.Equals(x.UserLogin.Username, StringComparison.OrdinalIgnoreCase) &&
                                                                                       userLogin.Password.Equals(x.UserLogin.Password));
                if (employee != null && employee.UserLogin != null)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("da3d80f0-b79b-4179-b791-cc4e8bfdeb2b"));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new Claim("UserID", employee.EmployeeID.ToString()),
                        new Claim("Username", employee.UserLogin.Username),
                        new Claim("UserType", ((int)EnumUserType.Employee).ToString())
                    };

                    var token = new JwtSecurityToken(
                        issuer: "ml_issuer",
                        audience: "ml_audience",
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: credentials
                    );

                    string userToken = new JwtSecurityTokenHandler().WriteToken(token);

                    if (Config.UseCookie)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = DateTime.UtcNow.AddDays(1)
                        };

                        Response.Cookies.Append("AuthToken", userToken, cookieOptions);
                    } else
                    {
                        Response.Cookies.Delete("AuthToken");
                    }

                    result.Data = userToken;
                } else
                {
                    result.Success = false;
                    result.ErrorCode = EnumApplicationErrorCode.InvalidLoginInfo;
                    result.ErrorMsg = "Tên đăng nhập hoặc mật khẩu không chinh xác";
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Thực hiện đăng nhập khách hàng
        /// </summary>
        /// <param name="users">Dữ liệu tài khoản</param>
        /// <returns></returns>
        [HttpPost("LoginCustomer")]
        public MLActionResult LoginCustomer(UserLogin userLogin)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                Customer? customer = _context.Customer.Include(x => x.UserLogin).FirstOrDefault(x => x.UserLogin != null &&
                                                                                       userLogin.Username.Equals(x.UserLogin.Username, StringComparison.OrdinalIgnoreCase) &&
                                                                                       userLogin.Password.Equals(x.UserLogin.Password));
                if (customer != null && customer.UserLogin != null)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("da3d80f0-b79b-4179-b791-cc4e8bfdeb2b"));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new Claim("UserID", customer.CustomerID.ToString()),
                        new Claim("Username", customer.CustomerName),
                        new Claim("UserType", ((int)EnumUserType.Customer).ToString())
                    };

                    var token = new JwtSecurityToken(
                        issuer: "ml_issuer",
                        audience: "ml_audience",
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: credentials
                    );

                    string userToken = new JwtSecurityTokenHandler().WriteToken(token);

                    if (Config.UseCookie)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = DateTime.UtcNow.AddDays(1)
                        };
                        Response.Cookies.Append("AuthToken", userToken, cookieOptions);
                    } else
                    {
                        Response.Cookies.Delete("AuthToken");
                    }

                    result.Data = userToken;
                }
                else
                {
                    result.Success = false;
                    result.ErrorCode = EnumApplicationErrorCode.InvalidLoginInfo;
                    result.ErrorMsg = "Tên đăng nhập hoặc mật khẩu không chinh xác";
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Logout")]
        public MLActionResult Logout()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                if (!Config.UseCookie)
                {
                    Response.Cookies.Delete("AuthToken");
                }

                //Session.Token = string.Empty;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        [Authorize]
        [HttpGet("GetUserData")]
        public MLActionResult GetUserData()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                if (!Config.UseCookie)
                {
                    Response.Cookies.Delete("AuthToken");
                }

                string strUserID = User.Claims.FirstOrDefault(x => x.Type == "UserID")?.Value ?? "";
                _ = Guid.TryParse(strUserID, out Guid userID);
                string strUserName = User.Claims.FirstOrDefault(x => x.Type == "Username")?.Value ?? "";
                string strUserType = User.Claims.FirstOrDefault(x => x.Type == "UserType")?.Value ?? "";
                _ = EnumUserType.TryParse(strUserType, out EnumUserType userType);

                SessionInterface sessionInterface = new ()
                {
                    UserType = userType,
                    UserName = strUserName,
                };

                switch (userType)
                {
                    case EnumUserType.Employee:
                        sessionInterface.UserData = _context.Employee.FirstOrDefault(e => e.EmployeeID == userID);
                        break;
                    case EnumUserType.Customer:
                        sessionInterface.UserData = _context.Customer.FirstOrDefault(e => e.CustomerID == userID);
                        break;
                }

                result.Data = sessionInterface;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
