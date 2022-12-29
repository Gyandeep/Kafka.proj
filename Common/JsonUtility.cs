using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Common
{
    public static class JsonUtility
    {
        public static string SerializeMessage<T>(T message)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>() { new StringEnumConverter() },
                TypeNameHandling = TypeNameHandling.Auto
            };

            return JsonConvert.SerializeObject(message, Formatting.None, setting);
        }

        public static T DeserializeMessage<T>(string message)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>() { new StringEnumConverter() },
                TypeNameHandling = TypeNameHandling.Auto
            };

            return JsonConvert.DeserializeObject<T>(message, setting);
        }
    }
}
