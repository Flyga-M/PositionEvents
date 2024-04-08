using PositionEvents.Area;
using System;

namespace PositionEvents
{
    public interface IPositionHandler : IDisposable
    {
        /// <summary>
        /// The amount of <see cref="IBoundingObject">IBoundingObjects</see> that were added to 
        /// this <see cref="IPositionHandler"/>.
        /// </summary>
        /// <returns>The amount of <see cref="IBoundingObject">IBoundingObjects</see> that were added to 
        /// this <see cref="IPositionHandler"/>.</returns>
        int Count();
        
        /// <summary>
        /// Updates the <see cref="IPositionHandler"/> evaluation based on the given <paramref name="positionData"/>.
        /// </summary>
        /// <param name="positionData"></param>
        void Update(PositionData positionData);

        /// <summary>
        /// Adds the given <paramref name="boundingObject"/> as a new area inside the given map. 
        /// The <paramref name="callback"/> will be called, when the player position moves inside or outside the 
        /// <paramref name="boundingObject"/>.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="boundingObject"></param>
        /// <param name="callback"></param>
        void AddArea(int mapId, IBoundingObject boundingObject, Action<PositionData, bool> callback);

        /// <summary>
        /// Removes the given <paramref name="boundingObject"/> area from the given map.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the given <paramref name="boundingObject"/> exists for the given map and was 
        /// successfully removed. Otherwise false.</returns>
        bool RemoveArea(int mapId, IBoundingObject boundingObject);

        /// <summary>
        /// Clears all <see cref="IBoundingObject">IBoundingObjects</see> and corresponding data.
        /// </summary>
        void Clear();
    }
}
