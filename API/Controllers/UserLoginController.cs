using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Extensions;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

            public string Token { get; set; } = string.Empty;

            public List<Setting> Settings { get; set; } = [];
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
                    customer.UserLogin.Password = PasswordHasherService.HashPassword(customer.UserLogin.Password);
                }

                Customer? dbCustomer = _context.Customer.FirstOrDefault(c =>
                    (!string.IsNullOrEmpty(customer.PhoneNumber) && c.PhoneNumber == customer.PhoneNumber) ||
                    (!string.IsNullOrEmpty(customer.Email) && c.Email == customer.Email)
                );

                if (dbCustomer != null)
                {
                    UserLogin? dbUserLogin = _context.UserLogin.FirstOrDefault(ul => ul.CustomerID == dbCustomer.CustomerID);
                    if (dbUserLogin != null)
                    {
                        // Nếu đã có bản ghi Customer và cả bản ghi UserLogin tức là tài khoản khách hàng đã được tạo
                        result.Success = false;
                        result.ErrorMsg = "Số điện thoại hoặc email khách hàng đã tồn tại.";
                        return result;
                    } else
                    {
                        // Nếu chỉ có bản ghi Customer mà không có UserLogin (Tạo bởi nhân viên) thì cập nhật thêm UserLogin
                        dbCustomer.CustomerName = customer.CustomerName;
                        dbCustomer.PhoneNumber = customer.PhoneNumber;
                        dbCustomer.Email = customer.Email;
                        dbCustomer.UserLogin = customer.UserLogin;

                        _context.Entry(dbCustomer).State = EntityState.Modified;
                    }
                } else
                {
                    _context.Customer.Add(customer);
                }

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
            MLActionResult result = new();

            try
            {
                Employee? employee = _context.Employee.Include(x => x.UserLogin).FirstOrDefault(x => x.UserLogin != null && 
                                                                                       userLogin.Username.Equals(x.EmployeeCode, StringComparison.OrdinalIgnoreCase));
                if (employee != null && employee.UserLogin != null && PasswordHasherService.VerifyPassword(userLogin.Password, employee.UserLogin.Password))
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
                            Secure = false,
                            SameSite = SameSiteMode.None,
                            Expires = DateTime.UtcNow.AddDays(1)
                        };

                        Response.Cookies.Append("AuthToken", userToken, cookieOptions);
                    } else
                    {
                        Response.Cookies.Delete("AuthToken");
                    }

                    employee.UserLogin.Token = userToken;
                    _context.Entry(employee.UserLogin).State = EntityState.Modified;

                    if (_context.SaveChanges() > 0)
                    {
                        result.Success = true;
                        result.Data = new
                        {
                            UserID = employee.EmployeeID,
                            Token = userToken
                        };
                    }
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
            MLActionResult result = new();

            try
            {
                Customer? dbCustomer = _context.Customer.FirstOrDefault(c => c.PhoneNumber == userLogin.Username || c.Email == userLogin.Username);

                if (dbCustomer != null)
                {
                    UserLogin? dbUserLogin = _context.UserLogin.FirstOrDefault(ul => ul.CustomerID == dbCustomer.CustomerID);
                    if (dbUserLogin == null)
                    {
                        // Nếu như đã có bản ghi Customer mà chưa có bản ghi UserLogin (Tạo bởi nhân viên) tức là chưa thiết lập mật khẩu
                        result.Success = false;
                        result.ErrorMsg = "Bạn chưa thiết lập mật khẩu. Vui lòng đăng ký để tạo tài khoản.";
                        return result;
                    } else
                    {
                        // Nếu đúng mật khẩu thì đăng nhập
                        if (PasswordHasherService.VerifyPassword(userLogin.Password, dbUserLogin.Password))
                        {
                            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("da3d80f0-b79b-4179-b791-cc4e8bfdeb2b"));
                            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                            var claims = new[]
                            {
                                new Claim("UserID", dbCustomer.CustomerID.ToString()),
                                new Claim("Username", dbCustomer.CustomerName),
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
                                    Secure = false,
                                    SameSite = SameSiteMode.None,
                                    Expires = DateTime.UtcNow.AddDays(1)
                                };
                                Response.Cookies.Append("AuthToken", userToken, cookieOptions);
                            }
                            else
                            {
                                Response.Cookies.Delete("AuthToken");
                            }

                            dbUserLogin.Token = userToken;
                            _context.Entry(dbUserLogin).State = EntityState.Modified;

                            if (_context.SaveChanges() > 0)
                            {
                                result.Success = true;
                                result.Data = new
                                {
                                    UserID = dbCustomer.CustomerID,
                                    Token = userToken
                                };
                            }
                            return result;
                        }
                    }
                }

                result.Success = false;
                result.ErrorMsg = "Tên đăng nhập hoặc mật khẩu không chinh xác";
                return result;
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
                string strUserID = User.Claims.FirstOrDefault(x => x.Type == "UserID")?.Value ?? "";
                if (Guid.TryParse(strUserID, out Guid userID))
                {
                    var userLoginData = _context.UserLogin.FirstOrDefault(ul => ul.EmployeeID == userID || ul.CustomerID == userID);
                    if (userLoginData != null)
                    {
                        userLoginData.Token = "";
                        _context.Entry(userLoginData).State = EntityState.Modified;

                        if (_context.SaveChanges() > 0)
                        {
                            result.Success = true;
                        }
                    }
                }

                Response.Cookies.Delete("AuthToken");

                //Session.Token = string.Empty;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        [HttpGet("GetUserData")]
        public MLActionResult GetUserData(string? userID = "")
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                // Lấy danh sách thiết lập
                SessionInterface sessionInterface = new SessionInterface
                {
                    Settings = _context.Setting.ToList()
                };
                result.Data = sessionInterface;

                if (!Guid.TryParse(userID, out Guid gUserID))
                {
                    result.Success = false;
                    return result;
                }
                string? token = _context.UserLogin.FirstOrDefault(ul => ul.EmployeeID == gUserID || ul.CustomerID == gUserID)?.Token;
                // Validate token
                if (string.IsNullOrEmpty(token))
                {
                    result.Success = false;
                    return result;
                }

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    tokenHandler.ValidateToken(token, CommonValue.TokenValidationParameters, out _);

                    string strUserName = User.Claims.FirstOrDefault(x => x.Type == "Username")?.Value ?? "";
                    string strUserType = User.Claims.FirstOrDefault(x => x.Type == "UserType")?.Value ?? "";
                    _ = EnumUserType.TryParse(strUserType, out EnumUserType userType);

                    sessionInterface.UserType = userType;
                    sessionInterface.UserName = strUserName;

                    switch (userType)
                    {
                        case EnumUserType.Employee:
                            var employeeData = _context.Employee.Include(e => e.Roles).ThenInclude(r => r.Permissions).FirstOrDefault(e => e.EmployeeID == gUserID);
                            employeeData.RemoveCircularReferences();
                            sessionInterface.UserData = employeeData;

                            sessionInterface.Token = _context.UserLogin.FirstOrDefault(ul => ul.EmployeeID == gUserID)?.Token ?? "";
                            break;
                        case EnumUserType.Customer:
                            var customerData = _context.Customer.FirstOrDefault(e => e.CustomerID == gUserID);

                            sessionInterface.Token = _context.UserLogin.FirstOrDefault(ul => ul.CustomerID == gUserID)?.Token ?? "";
                            customerData.RemoveCircularReferences();
                            sessionInterface.UserData = customerData;
                            break;
                    }

                    result.Data = sessionInterface;
                }
                catch (Exception)
                {
                    result.Success = false;
                    Response.Cookies.Delete("AuthToken");
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("ChangePassword")]
        public MLActionResult ChangePassword(UserLogin userLogin)
        {
            MLActionResult result = new();

            try
            {
                UserLogin? dbUserLogin = _context.UserLogin.FirstOrDefault(ul =>
                    ((userLogin.EmployeeID.HasValue && ul.EmployeeID == userLogin.EmployeeID) || (userLogin.CustomerID.HasValue && ul.CustomerID == userLogin.CustomerID))
                );

                if (dbUserLogin != null)
                {
                    if (!PasswordHasherService.VerifyPassword(userLogin.OldPassword, dbUserLogin.Password))
                    {
                        result.ErrorMsg = "Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại.";
                        return result;
                    }

                    dbUserLogin.Password = PasswordHasherService.HashPassword(userLogin.Password);
                    dbUserLogin.Token = "";

                    _context.Entry(dbUserLogin).State = EntityState.Modified;
                    result.Success = _context.SaveChanges() > 0;

                    if (result.Success)
                    {
                        Response.Cookies.Delete("AuthToken");
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
