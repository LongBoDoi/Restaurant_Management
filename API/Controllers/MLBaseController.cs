using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MLBaseController<IMLEntity>(ApplicationDBContext context) : ControllerBase where IMLEntity : class
    {
        #region Properties
        protected readonly ApplicationDBContext _context = context;
        protected readonly DbSet<IMLEntity> _entities = context.Set<IMLEntity>();
        #endregion

        #region Constructor
        #endregion

        #region Common methods

        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetDataPaging")]
        public virtual MLActionResult GetDataPaging(int page, int itemsPerPage)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = new
                {
                    Data = _entities.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList(),
                    TotalCount = _entities.Count()
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("SaveChanges")]
        public MLActionResult SaveChanges(IMLEntity entity)
        {
            MLActionResult result = new();

            try
            {
                if (entity is not MLEntity)
                {
                    result.ErrorMsg = "Bảng không tồn tại trong DB.";
                    return result;
                }

                EnumEditMode? editMode = (EnumEditMode?)typeof(IMLEntity).GetProperty("EditMode")?.GetValue(entity);
                if (editMode.HasValue)
                {
                    switch (editMode)
                    {
                        case EnumEditMode.Add:
                            var keyProperty = typeof(IMLEntity).GetProperties()
                                                                .FirstOrDefault(prop => prop.GetCustomAttribute<KeyAttribute>() != null);
                            keyProperty?.SetValue(entity, Guid.NewGuid());
                            _entities.Add(entity);
                            break;
                        case EnumEditMode.Edit:
                            _context.Entry(entity).State = EntityState.Modified;
                            break;
                        case EnumEditMode.Delete:
                            _context.Entry(entity).State = EntityState.Deleted;
                            break;
                    }
                }

                result.Success = _context.SaveChanges() > 0;
                if (result.Success)
                {
                    this.AfterSaveSuccess(entity);
                }
                if (editMode != EnumEditMode.Delete)
                {
                    result.Data = entity;
                }
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Xử lý sau khi save thành công
        /// </summary>
        public virtual void AfterSaveSuccess(IMLEntity entity)
        {
        }
        #endregion
    }
}
