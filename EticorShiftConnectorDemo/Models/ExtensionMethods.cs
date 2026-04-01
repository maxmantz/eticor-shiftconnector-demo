using System.Text;

namespace EticorShiftConnectorDemo.Models
{
    internal static class ExtensionMethods
    {
        public static string ToQueryParameters(this object obj)
        {
            System.Reflection.PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder query = new StringBuilder();
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                object? value = property.GetValue(obj);
                if (value == null)
                {
                    continue; // Skip null values
                }

                if (property.PropertyType.IsArray || (property.PropertyType.IsGenericType && property.PropertyType.GetElementType() != null))
                {
                    if (value is System.Collections.IEnumerable enumerable and not string)
                    {
                        foreach (object? item in enumerable)
                        {
                            if (item != null)
                            {
                                _ = query.Append($"{property.Name}={item}&");
                            }
                        }
                    }
                }
                else if (property.PropertyType == typeof(DateTime?) && value is DateTime dateTime)
                {
                    _ = query.Append($"{property.Name}={dateTime:yyyy-MM-ddTHH:mm:ssZ}&");
                }
                else
                {
                    _ = query.Append($"{property.Name}={value}&");
                }
            }
            return query.ToString().TrimEnd('&');
        }
    }
}