using Newtonsoft.Json;
using System.Collections.Generic;

namespace BracketPairColorizer.Settings
{
    public static class JsonExtensions
    {
        public static bool ReadExpected(this JsonTextReader reader, JsonToken expected)
        {
            if ( reader.Read() )
            {
                return reader.TokenType == expected;
            }

            return false;
        }

        public static bool ReadStartObject(this JsonTextReader reader)
        {
            return reader.ReadExpected(JsonToken.StartObject);
        }

        public static bool ReadEndObject(this JsonTextReader reader)
        {
            return reader.ReadExpected(JsonToken.EndObject);
        }

        public static bool ReadStartArray(this JsonTextReader reader)
        {
            return reader.ReadExpected(JsonToken.StartArray);
        }

        public static bool ReadEndArray(this JsonTextReader reader)
        {
            return reader.ReadExpected(JsonToken.EndArray);
        }

        public static string ReadPropertyName(this JsonTextReader reader)
        {
            if ( reader.ReadExpected(JsonToken.PropertyName) )
            {
                return reader.Value as string;
            }

            return null;
        }

        public static List<T> ListFromJson<T>(this string json)
        {
            var list = JsonConvert.DeserializeObject<List<T>>(json);

            if ( list == null ) { list = new List<T>(); }

            return list;
        }

        public static string ListToJson<T>(this IEnumerable<T> elements)
        {
            return JsonConvert.SerializeObject(elements);
        }
    }
}
