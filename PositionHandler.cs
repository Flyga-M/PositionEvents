using PositionEvents.Area;
using PositionEvents.Implementation.Handlers;
using System;

namespace PositionEvents
{
    public class PositionHandler : IPositionHandler
    {
        private bool _disposed;
        
        private readonly IPositionHandler _internalHandler;

        public PositionHandler(EventHandler<PositionData> mapChanged, IPositionHandler internalHandler = null)
        {
            if (internalHandler == null)
            {
                internalHandler = new PositionOcTree(mapChanged);
            }

            _internalHandler = internalHandler;
        }

        public int Count()
        {
            return _internalHandler.Count();
        }

        public void AddArea(int mapId, IBoundingObject boundingObject, Action<PositionData, bool> callback)
        {
            _internalHandler.AddArea(mapId, boundingObject, callback);
        }

        public void Clear()
        {
            _internalHandler.Clear();
        }

        public bool RemoveArea(int mapId, IBoundingObject boundingObject)
        {
            return _internalHandler.RemoveArea(mapId, boundingObject);
        }

        public void Update(PositionData positionData)
        {
            _internalHandler.Update(positionData);
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
                    _internalHandler.Dispose();
                }

                _disposed = true;
            }
        }

        ~PositionHandler()
        {
            Dispose(false);
        }
    }
}
