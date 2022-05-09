// This is a placeholder.
// This does not work.
// As we store the Starting Items in a dictionary where the key is a Starting Item object, we need to follow the factory pattern instead. https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-7-0

using System;
using System.Text.Json;

namespace StartingItemsGUI
{
    public class StartingItemJsonConverter : System.Text.Json.Serialization.JsonConverter<StartingItem>
    {
        public override StartingItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new StartingItem(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, StartingItem value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
