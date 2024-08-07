﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PositionEvents.Area;
using System.Collections.Concurrent;

namespace PositionEvents.Implementation.Handlers
{
    public class PositionOcTree : IPositionHandler
    {
        private bool _disposed;
        
        private const int MAX_OBJECTS_PER_NODE = 8;

        private int _previousMapId = -1;

        private EventHandler<PositionData> _mapChanged;

        private readonly Dictionary<int, OcTree.OcTree> _ocTrees = new Dictionary<int, OcTree.OcTree>();

        private readonly ConcurrentDictionary<IBoundingObject, ObjectData> _objectData = new ConcurrentDictionary<IBoundingObject, ObjectData>();

        public PositionOcTree(EventHandler<PositionData> mapChanged)
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

            if (!_ocTrees.ContainsKey(previousMapId))
            {
                return;
            }

            foreach (ObjectData data in _objectData
                .Where(keyValuePair => _ocTrees[previousMapId].Flatten().Contains(keyValuePair.Key))
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

            if (!_ocTrees.ContainsKey(mapId))
            {
                return;
            }

            // copy the current state of _objectData, in case it get's modified while update is still ongoing.
            Dictionary<IBoundingObject, ObjectData> currentObjectData = _objectData.ToDictionary(current => current.Key, current => current.Value);

            IBoundingObject[] containingBoundingObjects = _ocTrees[mapId].GetContaining(position);

            foreach(IBoundingObject containingBoundingObject in containingBoundingObjects)
            {
                if (!currentObjectData.ContainsKey(containingBoundingObject))
                {
                    throw new ArgumentOutOfRangeException(nameof(containingBoundingObject), "Can't invoke callback or change " +
                        "state for bounding object, that " +
                        "is not contained in _objectData.");
                }

                ObjectData data = currentObjectData[containingBoundingObject];

                data.SetState(true, positionData);
            }

            IEnumerable<IBoundingObject> leftBoundingObjects = currentObjectData
                .Where(keyValuePair => keyValuePair.Value.IsContained)
                .Select(keyValuePair => keyValuePair.Key)
                .Except(containingBoundingObjects);

            foreach(IBoundingObject leftBoundingObject in leftBoundingObjects)
            {
                if (!currentObjectData.ContainsKey(leftBoundingObject))
                {
                    throw new ArgumentOutOfRangeException(nameof(leftBoundingObject), "Can't invoke callback or change " +
                        "state for bounding object, that " +
                        "is not contained in _objectData.");
                }

                ObjectData data = currentObjectData[leftBoundingObject];

                data.SetState(false, positionData);
            }
        }

        public void AddArea(int mapId, IBoundingObject boundingObject, Action<PositionData, bool> callback)
        {
            if (!_ocTrees.ContainsKey(mapId))
            {
                _ocTrees[mapId] = new OcTree.OcTree(MAX_OBJECTS_PER_NODE, boundingObject.Bounds);
            }

            // Safety margin for float errors.
            _ocTrees[mapId].ResizeToInclude(boundingObject.Bounds.GetGrown(1));

            if (!_ocTrees[mapId].Add(boundingObject))
            {
                throw new InvalidOperationException("Unable to add containingBoundingObject to ocTree, because resize was " +
                    "not successfull. " + _ocTrees[mapId].Bounds + " vs. " + boundingObject.Bounds);
            }

            _objectData[boundingObject] = new ObjectData(callback);
        }

        public bool RemoveArea(int mapId, IBoundingObject boundingObject)
        {
            if (!_ocTrees.ContainsKey(mapId))
            {
                return false;
            }

            if (!_ocTrees[mapId].Remove(boundingObject))
            {
                return false;
            }

            return _objectData.TryRemove(boundingObject, out ObjectData _);
        }

        public void Clear()
        {
            foreach(OcTree.OcTree tree in _ocTrees.Values)
            {
                tree.Clear();
            }

            _ocTrees.Clear();
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

        ~PositionOcTree()
        {
            Dispose(false);
        }
    }
}
