using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Expressions;
using API.ML.CustomAtrributes;

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
}
