using Kinvo.Utilities.Util;
using Newtonsoft.Json;
using System;

namespace LogglyAPI.Converters
{
    public class EpochToDateTimeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(long);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                if (reader.Value == null)
                    return null;

                return DateTimeParser.ConvertUnixEpochTimeToDateTime(Convert.ToInt64(reader.Value));
            }

            throw new JsonSerializationException("Unexpected token type: " + reader.TokenType.ToString());
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
