using API.ML.BOBase;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace API.ML.Extensions
{
    public static class ObjectExtension
    {
        public static void AssignValuesFrom<IEntity>(this IEntity entity, IEntity sourceEntity) where IEntity : MLEntity
        {
            var properties = typeof(IEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<KeyAttribute>() == null
                    && !typeof(MLEntity).IsAssignableFrom(p.PropertyType)
                    && !typeof(IEnumerable<MLEntity>).IsAssignableFrom(p.PropertyType)
                );

            foreach (var property in properties)
            {
                var newValue = property.GetValue(sourceEntity);

                property.SetValue(entity, newValue);
            }
        }
    }
}
