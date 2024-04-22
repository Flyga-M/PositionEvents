using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area
{
    public class BoundingObjectGroupUnion : IBoundingObjectGroup
    {
        private readonly List<IBoundingObject> _content;

        private BoundingBox _bounds;

        private readonly bool _isComplex;

        public IEnumerable<IBoundingObject> Content => _content.ToArray();

        [JsonIgnore]
        public int Count => _content.Count;

        [JsonIgnore]
        public BoundingBox Bounds => _bounds;

        public bool IsComplex => _isComplex;

        public BoundingObjectGroupUnion(bool isComplex = true)
        {
            _content = new List<IBoundingObject>();

            CalculateBounds();

            _isComplex = isComplex;
        }

        [JsonConstructor]
        public BoundingObjectGroupUnion(IEnumerable<IBoundingObject> content, bool isComplex = true)
        {
            _content = content.ToList();

            CalculateBounds();

            _isComplex = isComplex;
        }

        public bool Contains(Vector3 position)
        {
            return _content.Any(boundingObject => boundingObject.ContainsEfficent(position));
        }

        private void CalculateBounds()
        {
            if (_content.Count == 0)
            {
                _bounds = new BoundingBox(Vector3.Zero, Vector3.Zero);
                return;
            }

            BoundingBox result = _content.First().Bounds;

            foreach (IBoundingObject boundingObject in _content.Skip(1))
            {
                result = BoundingBox.CreateMerged(result, boundingObject.Bounds);
            }

            _bounds = result;
        }

        /// <summary>
        /// <inheritdoc/> 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>.
        /// </summary>
        /// <param name="boundingObject"></param>
        public void Add(IBoundingObject boundingObject)
        {
            AddWithoutRecalculating(boundingObject);
            CalculateBounds();
        }

        private void AddWithoutRecalculating(IBoundingObject boundingObject)
        {
            if (_content.Contains(boundingObject))
            {
                return;
            }
            _content.Add(boundingObject);
        }

        /// <summary>
        /// <inheritdoc/> 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>.
        /// </summary>
        /// <param name="boundingObjects"></param>
        public void AddRange(IEnumerable<IBoundingObject> boundingObjects)
        {
            foreach (IBoundingObject boundingObject in boundingObjects)
            {
                AddWithoutRecalculating(boundingObject);
            }
            CalculateBounds();
        }

        public bool Remove(IBoundingObject boundingObject)
        {
            if (!_content.Contains(boundingObject))
            {
                return false;
            }

            bool removeEval = _content.Remove(boundingObject);

            CalculateBounds();

            return removeEval;
        }

        public void Clear()
        {
            _content.Clear();
            CalculateBounds();
        }
    }
}
