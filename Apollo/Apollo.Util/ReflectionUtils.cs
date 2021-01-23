namespace Apollo.Util
{
    public static class ReflectionUtils
    {
        public static T GetPropertyValue<T>(object obj, string propName)
        {
            return (T)obj.GetType().GetProperty(propName)?.GetValue(obj, null);
        }

    }
}
