using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area.JSON
{
    public class BasicConverter<TBase> : ReadOnlyBasicConverter<TBase>
    {
        public BasicConverter(Dictionary<string, Type> subTypes) : base(subTypes) { }

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
    }
}
