using System.Reflection;

namespace PubQuizBackend.Util
{
    public static class PropertyUpdater
    {
        public static void UpdateEntityFromDto<TDto, TEntity>(TDto dto, TEntity entity, params string[] excludedProperties)
        {
            var dtoProps = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var entityProps = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var dtoProp in dtoProps)
            {
                if (excludedProperties.Contains(dtoProp.Name))
                    continue;

                var entityProp = entityProps.FirstOrDefault(p =>
                    p.Name == dtoProp.Name &&
                    p.PropertyType == dtoProp.PropertyType &&
                    p.CanWrite);

                if (entityProp != null)
                {
                    var dtoValue = dtoProp.GetValue(dto);
                    var entityValue = entityProp.GetValue(entity);

                    if (!Equals(dtoValue, entityValue))
                    {
                        entityProp.SetValue(entity, dtoValue);
                    }
                }
            }
        }
    }
}
