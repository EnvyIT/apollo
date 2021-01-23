using System.Linq;

namespace Apollo.Util
{
    public static class ObjectExtension
    {
        public static string ToPrettyString(this object property)
        {
            return property == null ? 
                string.Empty:
                string.Join(" | ",
                property.GetType().GetProperties()
                    .Select(info => $"{info.Name} = {info.GetValue(property, null) ?? "(null)"}")
            );
        }
    }
}
