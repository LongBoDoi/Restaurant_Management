﻿using AngleSharp.Dom;
using API.ML.BO;
using API.ML.BOBase;
using API.ML.Common;
using API.ML.CustomAtrributes;
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
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetAll")]
        public virtual MLActionResult GetAll()
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                result.Data = _entities.ToList();
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu theo phân trang
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetDataPaging")]
        public virtual MLActionResult GetDataPaging(int page, int itemsPerPage, string? search)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                IEnumerable<IMLEntity> data = _entities.ToList();

                string? normalizedSearchTerm = search?.RemoveDiacritics().ToLower();
                if (!string.IsNullOrEmpty(normalizedSearchTerm))
                {

                    IEnumerable<PropertyInfo> nameFields = typeof(IMLEntity).GetProperties().Where(p => p.GetCustomAttribute<NameField>() != null);
                    if (nameFields.Any())
                    {
                        data = data.Where(e =>
                        {
                            foreach (PropertyInfo nameField in nameFields)
                            {
                                string? nameFieldValue = nameField.GetValue(e)?.ToString();
                                if (!string.IsNullOrEmpty(nameFieldValue) && nameFieldValue.RemoveDiacritics().ToLower().Contains(normalizedSearchTerm))
                                {
                                    return true;
                                }
                            }
                            return false;
                        });
                    }
                }

                result.Data = new
                {
                    Data = data.OrderByDescending(e => (e as MLEntity)?.CreatedDate).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList(),
                    TotalCount = data.Count()
                };
            }
            catch (Exception ex)
            {
                CommonFunction.HandleException(ex, result, _context);
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu theo ID
        /// </summary>
        /// <param name="page">Số trang</param>
        /// <param name="itemsPerPage">Kích thước trang</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetByID")]
        public virtual MLActionResult GetByID(Guid ID)
        {
            MLActionResult result = new()
            {
                Success = true
            };

            try
            {
                var keyProperty = typeof(IMLEntity).GetProperties()
                                                    .FirstOrDefault(prop => prop.GetCustomAttribute<KeyAttribute>() != null);

                result.Data = _entities.FirstOrDefault(x => keyProperty != null && keyProperty.GetValue(x) != null && (Guid?)keyProperty.GetValue(x) == ID);
                result.Success = result.Data != null;
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
        [HttpPost("SaveChangesMultiple")]
        public MLActionResult SaveChangesMultiple(List<IMLEntity> entities)
        {
            MLActionResult result = new();

            try
            {
                foreach (IMLEntity entity in entities)
                {
                    if (!BeforeSave(entity, result))
                    {
                        entities.Remove(entity);
                    }
                }

                if (!entities.Any())
                {
                    result.Success = false;
                    result.ErrorMsg = "Không có bản ghi nào để xử lý";
                    return result;
                }

                foreach (IMLEntity entity in entities)
                {
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
                }

                result.Success = _context.SaveChanges() > 0;
                if (result.Success)
                {
                    foreach (IMLEntity entity in entities)
                    {
                        this.AfterSaveSuccess(entity);
                    }
                    result.Data = entities;
                }
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
                if (!BeforeSave(entity, result))
                {
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
        protected virtual void AfterSaveSuccess(IMLEntity entity)
        {
        }

        protected virtual bool BeforeSave(IMLEntity entity, MLActionResult result)
        {
            return true;
        }
        #endregion
    }
}
