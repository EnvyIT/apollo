using Newtonsoft.Json;

namespace Apollo.Import.Util
{
    public static class JsonMapper
    {
        public static T Map<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
