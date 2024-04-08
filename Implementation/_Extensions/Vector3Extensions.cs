using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PositionEvents.Area;

namespace PositionEvents.Implementation
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Adds a value to the given <paramref name="axis"/> of this <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector3 AddSingle(this Vector3 vector, float value, Axis3 axis)
        {
            switch(axis)
            {
                case Axis3.X:
                    return new Vector3(vector.X + value, vector.Y, vector.Z);
                case Axis3.Y:
                    return new Vector3(vector.X, vector.Y + value, vector.Z);
                case Axis3.Z:
                    return new Vector3(vector.X, vector.Y, vector.Z + value);
                case Axis3.All:
                    return new Vector3(vector.X + value, vector.Y + value, vector.Z + value);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the minimal values from the given vectors.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>The <see cref="Vector3"/> with minimal values from the given vectors.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector3 Min(this Vector3 vector, IEnumerable<Vector3> values)
        {
            if (values.Count() < 1)
            {
                throw new ArgumentException("values must at least have 1 entry.", nameof(values));
            }
            Vector3 result = vector;

            foreach (Vector3 value in values)
            {
                result = Vector3.Min(result, value);
            }

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Vector3"/> that contains the maximal values from the given vectors.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>The <see cref="Vector3"/> with maximal values from the given vectors.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Vector3 Max(this Vector3 vector, IEnumerable<Vector3> values)
        {
            if (values.Count() < 1)
            {
                throw new ArgumentException("values must at least have 1 entry.", nameof(values));
            }
            Vector3 result = vector;

            foreach (Vector3 value in values)
            {
                result = Vector3.Max(result, value);
            }

            return result;
        }
    }
}
