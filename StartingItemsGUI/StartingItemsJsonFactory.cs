namespace StartingItemsGUI
{
    public class StartingItemsJsonFactory : System.Text.Json.Serialization.JsonConverterFactory
    {
        public override bool CanConvert(System.Type typeToConvert)
        {
            Log.LogDebug($"Checking if we can convert: {typeToConvert}");
            return typeToConvert == typeof(System.Collections.Generic.Dictionary<StartingItem, System.UInt32>);
        }

        public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type type, System.Text.Json.JsonSerializerOptions options)
        {
            Log.LogDebug($"Creating converter for type: {type}");
            Log.LogDebug($"Count: {type.GetGenericArguments().Length}");

            var keyType = type.GetGenericArguments()[0];
            Log.LogDebug($"Key type: {keyType}");
            var valueType = type.GetGenericArguments()[1];
            Log.LogDebug($"Value type: {valueType}");         

            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
            var activatorInstance = System.Activator.CreateInstance(typeof(DictionaryConverterInner<,>).MakeGenericType(new System.Type[] { keyType, valueType }), flags, binder: null, args: new object[] { options }, culture: null)!;
            var converter = (System.Text.Json.Serialization.JsonConverter)activatorInstance;

            return converter;
        }

        private class DictionaryConverterInner<TKey, TValue> : System.Text.Json.Serialization.JsonConverter<System.Collections.Generic.Dictionary<TKey, TValue>> where TKey : StartingItem
        {
            private readonly System.Text.Json.Serialization.JsonConverter<TValue> _valueConverter;
            private readonly System.Type _valueType = typeof(System.UInt32);

            public DictionaryConverterInner(System.Text.Json.JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _valueConverter = (System.Text.Json.Serialization.JsonConverter<TValue>)options.GetConverter(typeof(TValue));
            }

            public override System.Collections.Generic.Dictionary<TKey, TValue> Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
            {
                if (reader.TokenType != System.Text.Json.JsonTokenType.StartObject)
                {
                    throw new System.Text.Json.JsonException();
                }

                var dictionary = new System.Collections.Generic.Dictionary<TKey, TValue>();

                while (reader.Read())
                {
                    if (reader.TokenType == System.Text.Json.JsonTokenType.EndObject)
                    {
                        return dictionary;
                    }

                    // Get the key.
                    if (reader.TokenType != System.Text.Json.JsonTokenType.PropertyName)
                    {
                        throw new System.Text.Json.JsonException();
                    }

                    string propertyName = reader.GetString();
                    Log.LogDebug($"We have property name: {propertyName}");
                    var startingItem = new StartingItem(propertyName);

                    // Get the value.
                    TValue value;
                    if (_valueConverter != null)
                    {
                        reader.Read();
                        value = _valueConverter.Read(ref reader, _valueType, options)!;
                    }
                    else
                    {
                        value = System.Text.Json.JsonSerializer.Deserialize<TValue>(ref reader, options)!;
                    }

                    // Add to dictionary.
                    dictionary.Add((TKey)startingItem, value);
                }

                throw new System.Text.Json.JsonException();
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, System.Collections.Generic.Dictionary<TKey, TValue> dictionary, System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach ((TKey key, TValue value) in dictionary)
                {
                    var propertyName = key.ToString();
                    writer.WritePropertyName
                        (options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                    if (_valueConverter != null)
                    {
                        _valueConverter.Write(writer, value, options);
                    }
                    else
                    {
                        System.Text.Json.JsonSerializer.Serialize(writer, value, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
