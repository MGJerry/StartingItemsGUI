namespace StartingItemsGUI
{
    public class StartingItemsJsonFactory : System.Text.Json.Serialization.JsonConverterFactory
    {
        public override bool CanConvert(System.Type typeToConvert)
        {
            return typeToConvert.GetType() == typeof(System.Collections.Generic.Dictionary<StartingItem, uint>);
        }

        public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type type, System.Text.Json.JsonSerializerOptions options)
        {
            System.Type keyType = typeof(StartingItem);
            System.Type valueType = typeof(System.UInt32);

            return (System.Text.Json.Serialization.JsonConverter)System.Activator.CreateInstance(
                typeof(DictionaryEnumConverterInner<,>).MakeGenericType(
                    new System.Type[] { keyType, valueType }),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null)!;
        }
        private class DictionaryEnumConverterInner<TKey, TValue> :
            System.Text.Json.Serialization.JsonConverter<System.Collections.Generic.Dictionary<TKey, TValue>> where TKey : StartingItem
        {
            private readonly System.Text.Json.Serialization.JsonConverter<TValue> _valueConverter;
            private readonly System.Type _keyType;
            private readonly System.Type _valueType;

            public DictionaryEnumConverterInner(System.Text.Json.JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _valueConverter = (System.Text.Json.Serialization.JsonConverter<TValue>)options
                    .GetConverter(typeof(TValue));

                // Cache the key and value types.
                _keyType = typeof(TKey);
                _valueType = typeof(TValue);
            }

            public override System.Collections.Generic.Dictionary<TKey, TValue> Read(
                ref System.Text.Json.Utf8JsonReader reader,
                System.Type typeToConvert,
                System.Text.Json.JsonSerializerOptions options)
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

                    var startingItem = new StartingItem(reader.GetString());

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

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer,
                System.Collections.Generic.Dictionary<TKey, TValue> dictionary,
                System.Text.Json.JsonSerializerOptions options)
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
