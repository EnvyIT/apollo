using System;

namespace Apollo.Util
{
    public static class SingletonHelper
    {
        private static readonly object LockObject = new object();

        public static T GetInstance<T>(ref T instance, Func<T> createInstance)
        {
            if (createInstance == null)
            {
                throw new ArgumentNullException(nameof(createInstance));
            }

            lock (LockObject)
            {
                instance ??= createInstance();
            }

            return instance;
        }
    }
}