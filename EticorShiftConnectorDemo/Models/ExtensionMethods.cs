using System.Text;

namespace EticorShiftConnectorDemo.Models;

internal static class ExtensionMethods
{
    public static string ToQueryParameters(this object obj)
    {
        var properties = obj.GetType().GetProperties();
        var query = new StringBuilder();
        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            if (property.PropertyType.IsArray)
            {
                var array = (string[])value;
                foreach (var item in array)
                {
                    query.Append($"{property.Name}={item}&");
                }
            }
            else if (value != null)
            {
                query.Append($"{property.Name}={value}&");
            }
        }
        return query.ToString().TrimEnd('&');
    }
}
