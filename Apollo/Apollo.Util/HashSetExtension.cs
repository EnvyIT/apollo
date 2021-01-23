using System.Collections.Generic;

namespace Apollo.Util
{
    public static class HashSetExtension
    {

        public static bool AddAll<T>(this ISet<T> set, IEnumerable<T> items)
        {
            var allInserted = true;
            foreach (var item in items)
            {
                if (!set.Add(item))
                {
                    allInserted = false;
                }
            }
            return allInserted;
        }
    }
}
