using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Expressions;
using API.ML.CustomAtrributes;
using API.ML.BOBase;
using Newtonsoft.Json;

public static class QueryableExtensions
{
    public static IQueryable<T> WithAutoIncludes<T, IAttribute>(this IQueryable<T> query) where T : class where IAttribute : Attribute
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
            var property = Expression.Property(parameter, filter.Name);
            var constantValue = Convert.ChangeType(filter.Value, property.Type);
            var constant = Expression.Constant(constantValue, property.Type);

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
                default:
                    throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.");
            }

            combined = combined == null ? comparison : Expression.AndAlso(combined, comparison);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combined!, parameter);
        return query.Where(lambda);
    }
}
