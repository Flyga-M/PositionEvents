using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area.JSON
{
    public class BasicConverter<TBase> : JsonConverter
    {
        public const string TYPE_KEY = "$typeString";

        private readonly Dictionary<string, Type> _subTypes;
        private readonly Dictionary<Type, string> _subTypesSwapped;

        public BasicConverter(Dictionary<string, Type> subTypes)
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

        /// <summary>
        /// Adds a new <paramref name="subType"/> to the 
        /// <see cref="BasicConverter{TBase}"/>.
        /// </summary>
        /// <param name="typeString"></param>
        /// <param name="subType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(string typeString, Type subType)
        {
            if (string.IsNullOrWhiteSpace(typeString))
            {
                throw new ArgumentNullException("typeString must contain characters " +
                    "other than only whitespaces.", nameof(typeString));
            }
            if (subType == null)
            {
                throw new ArgumentNullException(nameof(subType));
            }
            if (!CanConvert(subType))
            {
                throw new ArgumentException("subType must be convertable by " +
                    $"the converter (subclass of or type {nameof(TBase)})", nameof(subType));
            }
            if (_subTypes.ContainsKey(typeString))
            {
                throw new ArgumentException("typeString is already used by the " +
                    "converter.", nameof(typeString));
            }
            if (_subTypes.ContainsValue(subType))
            {
                throw new ArgumentException("subType is already implemented by the " +
                    "converter.", nameof(subType));
            }

            _subTypes.Add(typeString, subType);
            _subTypesSwapped.Add(subType, typeString);
        }

        /// <summary>
        /// Attempts to add a new <paramref name="subType"/> to the
        /// <see cref="BasicConverter{TBase}"/>. May fail, if the parameters are null, 
        /// the <paramref name="subType"/> is not convertable by the 
        /// <see cref="BasicConverter{TBase}"/>, or either 
        /// <paramref name="typeString"/> or <paramref name="subType"/> are already 
        /// added to the <see cref="BasicConverter{TBase}"/>.
        /// </summary>
        /// <param name="typeString"></param>
        /// <param name="subType"></param>
        /// <returns>True, if the <paramref name="subType"/> was successfully added 
        /// to the <see cref="BasicConverter{TBase}"/>. Otherwise false.</returns>
        public bool TryAdd(string typeString, Type subType)
        {
            try
            {
                Add(typeString, subType);
            }
            catch
            {
                return false;
            }

            return true;
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
                || objectType == typeof(TBase);
        }

        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
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

            return JsonConvert.DeserializeObject(@base.ToString(), _subTypes[typeString]);

            // causes a recursive loop
            //return serializer.Deserialize(reader, _subTypes[typeString]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = value.GetType();

            if (!_subTypesSwapped.ContainsKey(type))
            {
                throw new NotImplementedException();
            }

            string typeString = _subTypesSwapped[type];

            JObject @base = JObject.FromObject(value);

            @base.Add(TYPE_KEY, typeString);

            serializer.Serialize(writer, @base, type);
        }
    }
}
