using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area.JSON
{
    public class ReadOnlyBasicConverter<TBase> : JsonConverter
    {
        public const string TYPE_KEY = "$typeString";

        protected readonly Dictionary<string, Type> _subTypes;
        protected readonly Dictionary<Type, string> _subTypesSwapped;

        public ReadOnlyBasicConverter(Dictionary<string, Type> subTypes)
        {
            if (subTypes == null)
            {
                throw new ArgumentNullException(nameof(subTypes));
            }
            if (!subTypes.Any())
            {
                throw new ArgumentException("subTypes must at least contain one " +
                    "element.", nameof(subTypes));
            }
            if (subTypes.Any(subType => !CanConvert(subType.Value)))
            {
                throw new ArgumentException("subTypes must all be convertable by " +
                    $"the converter (subclass of or type {nameof(TBase)}).", nameof(subTypes));
            }

            _subTypes = new Dictionary<string, Type>(subTypes);
            _subTypesSwapped = _subTypes.ToDictionary(keyValuePair => keyValuePair.Value, keyValuePair => keyValuePair.Key);
        }

        public ReadOnlyBasicConverter(ReadOnlyBasicConverter<TBase> converter)
        {
            _subTypes = new Dictionary<string, Type>(converter._subTypes);
            _subTypesSwapped = _subTypes.ToDictionary(keyValuePair => keyValuePair.Value, keyValuePair => keyValuePair.Key);
        }

        /// <summary>
        /// Determines whether the <paramref name="typeString"/> is already used by the 
        /// <see cref="BasicConverter{TBase}"/>.
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns>True, if the <see cref="BasicConverter{TBase}"/> contains 
        /// the <paramref name="typeString"/>. Otherwise false.</returns>
        public bool Contains(string typeString)
        {
            return _subTypes.ContainsKey(typeString);
        }

        /// <summary>
        /// Determines whether the <paramref name="subType"/> is already used by the 
        /// <see cref="BasicConverter{TBase}"/>.
        /// </summary>
        /// <param name="subType"></param>
        /// <returns>True, if the <see cref="BasicConverter{TBase}"/> contains 
        /// the <paramref name="subType"/>. Otherwise false.</returns>
        public bool Contains(Type subType)
        {
            return _subTypesSwapped.ContainsKey(subType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(TBase))
                || objectType == typeof(TBase)
                || typeof(TBase).IsAssignableFrom(objectType);
        }

        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            
            JObject @base = JObject.Load(reader);

            if (!@base.ContainsKey(TYPE_KEY))
            {
                throw new JsonSerializationException($"{typeof(TBase)} has no entry for " +
                    $"type : {TYPE_KEY}.");
            }

            string typeString = @base[TYPE_KEY].ToString();

            if (!_subTypes.ContainsKey(typeString))
            {
                throw new NotImplementedException($"Type indicator ({TYPE_KEY}) {typeString} " +
                    $"is not currently supported. This might be caused by a typo, or missing " +
                    $"implementation.");
            }

            // need to copy over converters, so properties that need converters can still be property converterted.
            IList<JsonConverter> converters = serializer.Converters.ToList(); // make copy
            converters.ToArray().Any(converter =>
            {
                if (converter.GetType() == this.GetType())
                {
                    // remove itself, to prevent a recursive loop
                    converters.Remove(converter);
                    return true;
                }
                return false;
            });

            JsonSerializerSettings setting = new JsonSerializerSettings() { Converters = converters };

            return JsonConvert.DeserializeObject(@base.ToString(), _subTypes[typeString], setting);

            // causes a recursive loop
            //return serializer.Deserialize(reader, _subTypes[typeString]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            
            Type type = value.GetType();

            if (!_subTypesSwapped.ContainsKey(type))
            {
                throw new NotImplementedException();
            }

            string typeString = _subTypesSwapped[type];

            // need to copy over converters, so properties that need converters can still be property converterted.
            IList<JsonConverter> converters = serializer.Converters.ToList(); // make copy
            converters.ToArray().Any(converter =>
            {
                if (converter.GetType() == this.GetType())
                {
                    // remove itself, to prevent a recursive loop
                    converters.Remove(converter);
                    return true;
                }
                return false;
            });
            JsonSerializerSettings setting = new JsonSerializerSettings() { Converters = converters };
            // need a new serializer to not create a recursive loop
            JsonSerializer newSerializer = JsonSerializer.Create(setting);

            JObject @base = JObject.FromObject(value, newSerializer);

            @base.Add(TYPE_KEY, typeString);

            serializer.Serialize(writer, @base, type);
        }
    }
}
