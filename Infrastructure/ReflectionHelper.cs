using System;

namespace Acme.Packages.Menu.Infrastructure
{
    /// <summary>
    /// Manejador seguro de reflexión para APIs de GeneXus
    /// </summary>
    internal static class ReflectionHelper
    {
        public static T GetPropertyValue<T>(object obj, string propertyName, T defaultValue = default(T))
        {
            if (obj == null) return defaultValue;

            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                if (property == null) return defaultValue;

                var value = property.GetValue(obj);
                if (value == null) return defaultValue;

                if (value is T directValue)
                    return directValue;

                // Intentar conversión
                if (typeof(T) == typeof(string))
                    return (T)(object)value.ToString();

                if (typeof(T) == typeof(DateTime) && DateTime.TryParse(value.ToString(), out DateTime dateValue))
                    return (T)(object)dateValue;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static System.Collections.IEnumerable GetEnumerableProperty(object obj, string propertyName)
        {
            if (obj == null) return null;

            try
            {
                var property = obj.GetType().GetProperty(propertyName);
                var value = property?.GetValue(obj);
                return value as System.Collections.IEnumerable;
            }
            catch
            {
                return null;
            }
        }
    }
}