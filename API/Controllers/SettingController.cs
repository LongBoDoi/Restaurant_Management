using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class SettingController(ApplicationDBContext context) : MLBaseController<Setting>(context)
    {
        /// <summary>
        /// Lấy danh sách thiết lập cho form Màn hình khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSettings")]
        public MLActionResult GetSettings()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                List<Setting> settings = _context.Setting.Where(s => CommonValue.CustomerScreenSettingKeys.Contains(s.SettingKey)).ToList();
                result.Data = settings;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách món hiển thị trên màn hình khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMenuItemsForCustomerScreen")]
        public MLActionResult GetMenuItemsForCustomerScreen()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                List<MenuItemCategory> resultData = [];

                Setting? customerMenuItemDisplayType = _context.Setting.FirstOrDefault(s => s.SettingKey == "DisplayMenuScreenByItemsForCustomerType");
                if (customerMenuItemDisplayType != null)
                {
                    List<MenuItem> lstMenuItems = [];

                    switch (customerMenuItemDisplayType.Value)
                    {
                        case 0:
                            lstMenuItems = _context.MenuItem.ToList();
                            break;
                        case 1:
                            Setting? menuItemSetting = _context.Setting.FirstOrDefault(s => s.SettingKey == "ListMenuScreenForCustomerItems");
                            if (menuItemSetting != null && !string.IsNullOrEmpty(menuItemSetting.SettingValue))
                            {
                                List<Guid> listItemIDs = JsonConvert.DeserializeObject<List<Guid>>(menuItemSetting.SettingValue) ?? [];

                                lstMenuItems = _context.MenuItem.Where(mi => listItemIDs.Contains(mi.MenuItemID)).ToList();
                            }
                            break;
                    }

                    if (lstMenuItems.Any())
                    {
                        List<MenuItemCategory> lstCategories = _context.MenuItemCategory.OrderBy(mic => mic.SortOrder).ToList();

                        foreach (MenuItemCategory category in lstCategories)
                        {
                            List<MenuItem> categoryItems = lstMenuItems.Where(mi => mi.MenuItemCategoryID == category.MenuItemCategoryID).ToList();
                            if (categoryItems.Any())
                            {
                                category.MenuItems = categoryItems;
                                resultData.Add(category);
                            }
                        }

                        resultData.Add(new MenuItemCategory
                        {
                            MenuItemCategoryName = "Khác",
                            MenuItems = lstMenuItems.Where(mi => !mi.MenuItemCategoryID.HasValue)
                        });
                    }
                }
                
                result.Data = resultData;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lưu danh sách thiết lập cho form Màn hình khách hàng
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("UpdateSettings")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public MLActionResult UpdateSettings([FromForm] string settings, [FromForm] IFormFile? introImage, [FromForm] List<IFormFile> menuImages)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                List <Setting> lstSettings = JsonConvert.DeserializeObject<List<Setting>>(settings) ?? [];

                Setting? introImageSetting = lstSettings.FirstOrDefault(s => s.SettingKey == "IntroImageUrl");
                if (introImage != null && introImageSetting != null)
                {
                    string newFileName = CommonFunction.SaveImage(introImage);
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        introImageSetting.SettingValue = newFileName;
                    }
                }

                Setting? menuImagesSetting = lstSettings.FirstOrDefault(s => s.SettingKey == "ListMenuScreenForCustomerImages");
                if (menuImages.Any() && menuImagesSetting != null)
                {
                    List<string> imageUrls = JsonConvert.DeserializeObject<List<string>>(menuImagesSetting.SettingValue) ?? [];

                    foreach (IFormFile menuImage in menuImages)
                    {
                        string newFileName = CommonFunction.SaveImage(menuImage);
                        if (!string.IsNullOrEmpty(newFileName))
                        {
                            imageUrls.Add(newFileName);
                        }
                    }

                    menuImagesSetting.SettingValue = JsonConvert.SerializeObject(imageUrls);
                }

                // Cập nhật bản ghi Setting
                foreach (Setting setting in lstSettings)
                {
                    _context.Entry(setting).State = EntityState.Modified;
                }

                result.Success = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }
    }
}
