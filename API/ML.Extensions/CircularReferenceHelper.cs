using API.ML.BOBase;
using System.Collections;
using System.Reflection;

namespace API.ML.Extensions
{
    public static class CircularReferenceHelper
    {
        public static void RemoveCircularReferences(this MLEntity? entity, Type? type = null, HashSet<Type>? visited = null)
        {
            if (entity == null) return;

            type ??= entity.GetType();
            visited ??= new HashSet<Type>();

            var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => typeof(MLEntity).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<MLEntity>).IsAssignableFrom(p.PropertyType));
            var mainProperties = properties.Where(p => type.IsAssignableFrom(p.PropertyType) ||
                typeof(IEnumerable).IsAssignableFrom(p.PropertyType) &&
                    p.PropertyType.IsGenericType &&
                    type.IsAssignableFrom(p.PropertyType.GetGenericArguments()[0])
            );

            if (mainProperties.Any())
            {
                foreach (var prop in mainProperties)
                {
                    if (prop.GetMethod == null || !prop.GetMethod.IsPublic)
                        continue;

                    var value = prop.GetValue(entity);
                    if (value == null) continue;

                    prop.SetValue(entity, null);
                }

                if (visited.Add(type))
                {
                    string result = $"Removed {type.Name} from {entity.GetType().Name}";
                }
                return;
            }

            var otherProperties = properties.Where(p => !mainProperties.Contains(p) && !visited.Contains(p.PropertyType));
            foreach (var prop in otherProperties)
            {
                var value = prop.GetValue(entity);
                if (value == null) continue;

                if (typeof(MLEntity).IsAssignableFrom(prop.PropertyType))
                {
                    RemoveCircularReferences(value as MLEntity, type, visited);
                    RemoveCircularReferences(value as MLEntity, visited: visited);
                }
                else if (typeof(IEnumerable<MLEntity>).IsAssignableFrom(prop.PropertyType))
                {
                    foreach (var item in (IEnumerable)value)
                    {
                        RemoveCircularReferences(item as MLEntity, type, visited: visited);
                        RemoveCircularReferences(item as MLEntity, visited: visited);
                    }
                }
            }
        }

        public static void RemoveCircularReferences(this IEnumerable<MLEntity>? entities)
        {
            if (entities == null || !entities.Any())
                return;

            foreach (MLEntity entity in entities)
            {
                entity.RemoveCircularReferences();
            }
        }

        public static void RemoveAllReferences(this MLEntity? entity)
        {
            if (entity == null) return;

            var referenceProperties = entity.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof(MLEntity).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<MLEntity>).IsAssignableFrom(p.PropertyType));

            if (referenceProperties.Any())
            {
                foreach (var property in referenceProperties)
                {
                    var value = property.GetValue(entity);
                    if (value != null)
                    {
                        property.SetValue(entity, null);
                    }
                }
            }
        }

        public static void RemoveAllReferences(this IEnumerable<MLEntity>? entities)
        {
            if (entities == null || !entities.Any()) return;

            foreach (MLEntity entity in entities)
            {
                RemoveAllReferences(entity);
            }
        }
    }
}
