using System;
using System.Linq;

namespace Apollo.Util
{
    public static class TypeExtension
    {
        public static bool IsAnyType(this Type sourceType, params Type[] destinationTypes)
        {
            return destinationTypes.Any(sourceType.IsAssignableFrom);
        }
    }
}
