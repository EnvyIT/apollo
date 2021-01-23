using Newtonsoft.Json;

namespace Apollo.Util
{
    public static class JsonMapper
    {
        public static T Map<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string Map<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}
