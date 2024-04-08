using System.Collections.Generic;

namespace PositionEvents.Area
{
    public interface IBoundingObjectGroup : IBoundingObject
    {
        /// <summary>
        /// The <see cref="IBoundingObject">IBoundingObjects</see> that are contained in this 
        /// <see cref="IBoundingObjectGroup"/>.
        /// </summary>
        IEnumerable<IBoundingObject> Content { get; }

        /// <summary>
        /// The amount of <see cref="IBoundingObject">IBoundingObjects</see> that are contained in this 
        /// <see cref="IBoundingObjectGroup"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds a <paramref name="boundingObject"/> to this <see cref="IBoundingObjectGroup"/>.
        /// </summary>
        /// <param name="boundingObject"></param>
        void Add(IBoundingObject boundingObject);

        /// <summary>
        /// Adds multiple <paramref name="boundingObjects"/> to this <see cref="IBoundingObjectGroup"/>.
        /// </summary>
        /// <param name="boundingObjects"></param>
        void AddRange(IEnumerable<IBoundingObject> boundingObjects);

        /// <summary>
        /// Removes a <see cref="IBoundingObject"/> from this <see cref="IBoundingObjectGroup"/>;
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the <paramref name="boundingObject"/> is contained in this 
        /// <see cref="IBoundingObjectGroup"/> and is successfully removed. Otherwise false.</returns>
        bool Remove(IBoundingObject boundingObject);

        /// <summary>
        /// Removes all <see cref="IBoundingObject">IBoundingObjects</see> from this 
        /// <see cref="IBoundingObjectGroup"/>.
        /// </summary>
        void Clear();
    }
}
