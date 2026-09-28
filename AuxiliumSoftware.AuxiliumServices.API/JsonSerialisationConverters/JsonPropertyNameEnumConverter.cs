using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.JsonSerialisationConverters
{
    internal sealed class JsonPropertyNameEnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private static readonly Dictionary<TEnum, string> ValueToName = new();
        private static readonly Dictionary<string, TEnum> NameToValue = new(StringComparer.Ordinal);

        static JsonPropertyNameEnumConverter()
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var value = (TEnum)field.GetValue(null)!;
                var wireName = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? field.Name;

                ValueToName.TryAdd(value, wireName);

                NameToValue.TryAdd(wireName, value);
                NameToValue.TryAdd(field.Name, value);
            }
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a string for {typeof(TEnum).Name}, got {reader.TokenType}.");

            return Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => writer.WriteStringValue(ToWireName(value));

        public override TEnum ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Parse(reader.GetString());

        public override void WriteAsPropertyName(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => writer.WritePropertyName(ToWireName(value));



        private static TEnum Parse(string? value)
        {
            if (value is not null && NameToValue.TryGetValue(value, out var result))
                return result;

            throw new JsonException($"'{value}' is not a valid value for {typeof(TEnum).Name}.");
        }

        private static string ToWireName(TEnum value)
            => ValueToName.TryGetValue(value, out var name) ? name : value.ToString();
    }
}
