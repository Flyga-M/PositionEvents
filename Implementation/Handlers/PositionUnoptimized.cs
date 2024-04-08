using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PositionEvents.Area;

namespace PositionEvents.Implementation.Handlers
{
    public class PositionUnoptimized : IPositionHandler
    {
        private bool _disposed;

        private int _previousMapId = -1;

        private EventHandler<PositionData> _mapChanged;

        private readonly Dictionary<int, List<IBoundingObject>> _objects = new Dictionary<int, List<IBoundingObject>>();

        private readonly Dictionary<IBoundingObject, ObjectData> _objectData = new Dictionary<IBoundingObject, ObjectData>();

        public PositionUnoptimized(EventHandler<PositionData> mapChanged)
        {
            mapChanged += OnMapChange;
            _mapChanged = mapChanged;
        }

        public int Count()
        {
            return _objectData.Count;
        }

        private void OnMapChange(object _, PositionData positionData)
        {
            int previousMapId = _previousMapId;
            _previousMapId = positionData.MapId;

            if (!_objects.ContainsKey(previousMapId))
            {
                return;
            }

            foreach (ObjectData data in _objectData
                .Where(keyValuePair => _objects[previousMapId].Contains(keyValuePair.Key))
                .Select(keyValuePair => keyValuePair.Value)
                .Where(data => data.IsContained))
            {
                data.SetState(false, positionData);
            }
        }

        public void Update(PositionData positionData)
        {
            int mapId = positionData.MapId;
            Vector3 position = positionData.Position;

            if (!_objects.ContainsKey(mapId))
            {
                return;
            }

            IBoundingObject[] allBoundingObjects = _objects[mapId].ToArray();

            foreach (IBoundingObject boundingObject in allBoundingObjects)
            {
                if (!_objectData.ContainsKey(boundingObject))
                {
                    throw new ArgumentOutOfRangeException(nameof(boundingObject), "Can't invoke callback or change " +
                        "state for bounding object, that " +
                        "is not contained in _objectData.");
                }

                bool isContained = boundingObject.ContainsEfficent(position);

                ObjectData data = _objectData[boundingObject];

                data.SetState(isContained, positionData);
            }
        }

        public void AddArea(int mapId, IBoundingObject boundingObject, Action<PositionData, bool> callback)
        {
            if (!_objects.ContainsKey(mapId))
            {
                _objects[mapId] = new List<IBoundingObject>();
            }

            _objects[mapId].Add(boundingObject);

            _objectData[boundingObject] = new ObjectData(callback);
        }

        public bool RemoveArea(int mapId, IBoundingObject boundingObject)
        {
            if (!_objects.ContainsKey(mapId))
            {
                return false;
            }

            if (!_objects[mapId].Remove(boundingObject))
            {
                return false;
            }

            return _objectData.Remove(boundingObject);
        }

        public void Clear()
        {
            foreach (List<IBoundingObject> boundingObjects in _objects.Values)
            {
                boundingObjects.Clear();
            }
            _objects.Clear();
            _objectData.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // NOOP
                }

                _mapChanged -= OnMapChange;

                _disposed = true;
            }
        }

        ~PositionUnoptimized()
        {
            Dispose(false);
        }
    }
}
