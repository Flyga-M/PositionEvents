using PositionEvents.Area;
using System;

namespace PositionEvents.Implementation
{
    public class ObjectData
    {
        private readonly Action<PositionData, bool> _callback;

        private bool _previousIsContained;
        private bool _isContained;
        
        private PositionData _positionData;

        public event EventHandler<bool> StateChanged;

        protected virtual void OnStateChange(bool isContained)
        {
            StateChanged?.Invoke(this, isContained);
            _callback?.Invoke(_positionData, isContained);
        }

        /// <summary>
        /// The callback that will be invoked, if the state changes.
        /// </summary>
        internal Action<PositionData, bool> Callback => _callback;

        /// <summary>
        /// Determines whether a given position is contained in the connected <see cref="IBoundingObject"/>.
        /// </summary>
        public bool IsContained
        {
            get
            {
                return _isContained;
            }
            private set
            {
                if (!Equals(_previousIsContained, value))
                {
                    _previousIsContained = value;
                    OnStateChange(value);
                }

                _isContained = value;
            }
        }

        public ObjectData(Action<PositionData, bool> callback)
        {
            _callback = callback;

            _previousIsContained = false;
            _isContained = false;
        }

        /// <summary>
        /// Sets the <see cref="IsContained"/> value for this <see cref="ObjectData"/>. If the value changed, 
        /// the <see cref="StateChanged"/> event will be called and the <see cref="Callback"/> will be 
        /// invoked with the given <paramref name="positionData"/>.
        /// </summary>
        /// <param name="isContained"></param>
        /// <param name="positionData"></param>
        internal void SetState(bool isContained, PositionData positionData)
        {
            _positionData = positionData;
            IsContained = isContained;
        }
    }
}
