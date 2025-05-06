using API.ML.BOBase;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{
    public static IQueryable<T> WithAutoIncludes<T, IAttribute>(this IQueryable<T> query) where T : MLEntity where IAttribute : Attribute
    {
        var entityType = typeof(T);
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.GetCustomAttribute<IAttribute>() != null)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, prop);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Include" &&
                                m.GetParameters().Length == 2 &&
                                m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                    .MakeGenericMethod(typeof(T), prop.PropertyType);

                if (includeMethod != null)
                {
                    IQueryable<T>? newQuery = (IQueryable<T>?)includeMethod.Invoke(null, new object[] { query, lambda });
                    if (newQuery != null)
                    {
                        query = newQuery;
                    }
                }
            }
        }

        return query;
    }

    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, string? search)
    {
        if (string.IsNullOrEmpty(search))
            return query;

        var filters = JsonConvert.DeserializeObject<List<MLFilterCondition>>(search);
        if (filters == null || !filters.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combined = null;

        foreach (var filter in filters)
        {
            if (filter.Value == null) continue;

            var property = GetNestedProperty(parameter, filter.Name);

            Expression constant;
            if (!filter.CompareProperties)
            {
                var constantValue = filter.Value;
                if (property.Type.IsEnum)
                {
                    constantValue = Enum.ToObject(property.Type, filter.Value);
                }
                else if (property.Type == typeof(Guid) || property.Type == typeof(Guid?))
                {
                    constantValue = Guid.Parse(filter.Value.ToString() ?? "");
                }
                else if (property.Type == typeof(DateTime) || property.Type == typeof(DateTime?))
                {
                    constantValue = DateTime.Parse(filter.Value.ToString() ?? "");
                }
                else
                {
                    constantValue = Convert.ChangeType(filter.Value, property.Type);
                }

                constant = Expression.Constant(constantValue, property.Type);
            } else
            {
                constant = GetNestedProperty(parameter, filter.Value.ToString() ?? "");
            }

            Expression comparison;

            switch (filter.Operator)
            {
                case "==":
                    comparison = Expression.Equal(property, constant);
                    break;
                case "!=":
                    comparison = Expression.NotEqual(property, constant);
                    break;
                case ">":
                    comparison = Expression.GreaterThan(property, constant);
                    break;
                case ">=":
                    comparison = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case "<":
                    comparison = Expression.LessThan(property, constant);
                    break;
                case "<=":
                    comparison = Expression.LessThanOrEqual(property, constant);
                    break;
                case "Contains":
                    comparison = Expression.Call(property, "Contains", null, constant);
                    break;
                case "EqualDate":
                    {
                        // Only works for DateTime or Nullable<DateTime>
                        if (property.Type != typeof(DateTime) && property.Type != typeof(DateTime?))
                            throw new NotSupportedException($"Operator 'EqualDate' is only supported on DateTime or Nullable<DateTime> properties.");

                        // Date property call: x.Property.Date
                        var propertyDate = Expression.Property(property, "Date");
                        var constantDate = Expression.Property(constant, "Date");

                        comparison = Expression.Equal(propertyDate, constantDate);
                        break;
                    }
                case "IN":
                    var values = (IEnumerable<object>)filter.Value;

                    // Convert each value in the list to correct type
                    var typedValues = values.Select(v => Convert.ChangeType(v, property.Type)).ToList();

                    var listConstant = Expression.Constant(typedValues);

                    comparison = Expression.Call(listConstant, "Contains", null, property);
                    break;
                default:
                    throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.");
            }

            combined = combined == null ? comparison : Expression.AndAlso(combined, comparison);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combined!, parameter);
        return query.Where(lambda);
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortStr) where T : MLEntity
    {
        if (string.IsNullOrEmpty(sortStr))
            return query;

        var sortConditions = JsonConvert.DeserializeObject<IEnumerable<MLSortCondition>>(sortStr);

        if (sortConditions == null)
            return query;

        bool isFirstOrder = true;
        foreach (var sort in sortConditions)
        {
            if (sort.Direction == "RANDOM")
            {
                query = query.OrderBy(x => EF.Functions.Random());
                continue;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, sort.Name);
            Expression propertyAccess = property;
            if (property.Type.IsValueType)
            {
                propertyAccess = Expression.Convert(property, typeof(object));
            }

            var keySelector = Expression.Lambda<Func<T, object>>(propertyAccess, parameter);

            switch (sort.Direction)
            {
                case "ASC":
                    if (isFirstOrder)
                    {
                        query = query.OrderBy(keySelector);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(keySelector);
                    }
                    break;
                case "DESC":
                    if (isFirstOrder)
                    {
                        query = query.OrderByDescending(keySelector);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(keySelector);
                    }
                    break;
            }
        }

        return ((IOrderedQueryable<T>)query).ThenByDescending(x => x.ModifiedDate);
    }

    private static MemberExpression GetNestedProperty(ParameterExpression parameter, string propertyName)
    {
        if (!propertyName.Contains('.'))
        {
            return Expression.PropertyOrField(parameter, propertyName);
        }

        string[] parts = propertyName.Split('.');
        Expression property = parameter;

        foreach (var part in parts)
        {
            property = Expression.PropertyOrField(property, part);
        }

        return (MemberExpression)property;
    }

}
