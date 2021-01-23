using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Apollo.Util
{
    public static class ConfigurationHelper
    {

        public static IConfigurationRoot ConfigurationRoot { get; set; }

        public static string[] GetValues(params string[] keys)
        {
            var values = new string[keys.Length];
            var nullKeys = new List<string>();
            for (var i = 0; i < keys.Length; ++i)
            {
                values[i] = ConfigurationRoot.GetValue<string>(keys[i]);
                if (values[i] == null)
                {
                    nullKeys.Add(keys[i]);
                }
            }
            return values.Any(v => v == null)
                ? throw new KeyNotFoundException($"Keys {nullKeys.Aggregate(string.Empty, (current, next) => current + ", " + next)} not found in settings file") : values;
        }

    }
}
