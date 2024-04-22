using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PositionEvents.Area
{
    public class BoundingObjectGroupDifference : IBoundingObjectGroup
    {
        private IBoundingObject _positive;

        private readonly List<IBoundingObject> _negatives;

        private BoundingBox _bounds;

        private readonly bool _isComplex;

        /// <summary>
        /// <inheritdoc/> 
        /// If <see cref="Positive"/> is not null, it's the first element of this <see cref="IEnumerable{IBoundingObject}"/>. 
        /// The others are the negative components (see <see cref="Negatives"/>).
        /// </summary>
        public IEnumerable<IBoundingObject> Content
        {
            get
            {
                List<IBoundingObject> result = new List<IBoundingObject>();
                if (_positive != null)
                {
                    result.Add(_positive);
                }
                result.AddRange(_negatives);

                return result;
            }
        }

        /// <summary>
        /// The positive component of this Difference.
        /// </summary>
        public IBoundingObject Positive => _positive;

        /// <summary>
        /// The <see cref="IBoundingObject">IBoundingObjects</see>, that are subtracted from the 
        /// <see cref="Positive"/> component.
        /// </summary>
        public IEnumerable<IBoundingObject> Negatives => _negatives.ToArray(); // .ToArray(), so a copy instead of the internal list is returned

        [JsonIgnore]
        public int Count
        {
            get
            {
                int count = 0;
                if (_positive != null)
                {
                    count++;
                }

                return _negatives.Count + count;
            }
        }

        /// <summary>
        /// <inheritdoc/> 
        /// Attention: Just returns the bounds of the <see cref="Positive"/> component. In many cases this is 
        /// not the smallest possible Bounding Box.
        /// </summary>
        [JsonIgnore]
        public BoundingBox Bounds => _bounds;

        public bool IsComplex => _isComplex;

        public BoundingObjectGroupDifference(bool isComplex = true)
        {
            _positive = null;
            _negatives = new List<IBoundingObject>();

            CalculateBounds();

            _isComplex = isComplex;
        }

        public BoundingObjectGroupDifference(IEnumerable<IBoundingObject> content, bool isComplex = true)
        {
            if (content.Any())
            {
                _positive = content.First();
                _negatives = content.Skip(1).ToList();
            }
            else
            {
                _positive = null;
                _negatives = new List<IBoundingObject>();
            }

            CalculateBounds();

            _isComplex = isComplex;
        }

        [JsonConstructor]
        public BoundingObjectGroupDifference(IBoundingObject positive, IEnumerable<IBoundingObject> negatives, bool isComplex = true)
        {
            _positive = positive;
            _negatives = negatives.ToList();

            CalculateBounds();

            _isComplex = isComplex;
        }

        public bool Contains(Vector3 position)
        {
            if (_positive == null)
            {
                return false;
            }

            return _positive.ContainsEfficent(position) && !_negatives.Any(boundingObject => boundingObject.ContainsEfficent(position));
        }

        /// <summary>
        /// Returns the bounds of the <see cref="Positive"/> component. In many cases this is 
        /// not the smallest possible Bounding Box.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CalculateBounds()
        {
            if (_positive == null)
            {
                _bounds = new BoundingBox(Vector3.Zero, Vector3.Zero);
                return;
            }

            _bounds = _positive.Bounds;
        }

        /// <summary>
        /// <inheritdoc/> 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>. If <see cref="Positive"/> is null, 
        /// the <see cref="IBoundingObject"/> will be set as the positive component.
        /// </summary>
        /// <param name="boundingObject"></param>
        public void Add(IBoundingObject boundingObject)
        {
            AddWithoutRecalculating(boundingObject);
            CalculateBounds();
        }

        private void AddWithoutRecalculating(IBoundingObject boundingObject)
        {
            if (_positive == boundingObject || _negatives.Contains(boundingObject))
            {
                return;
            }

            if (_positive == null)
            {
                _positive = boundingObject;
                return;
            }

            _negatives.Add(boundingObject);
        }

        /// <summary>
        /// <inheritdoc/> 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>. If <see cref="Positive"/> is null, 
        /// the first element of <paramref name="boundingObjects"/> will be set as the positive component.
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

        /// <summary>
        /// Adds a <paramref name="boundingObject"/> to the negative component of the 
        /// <see cref="BoundingObjectGroupDifference"/>. 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>.
        /// </summary>
        /// <param name="boundingObject"></param>
        public void AddNegative(IBoundingObject boundingObject)
        {
            AddNegativeWithoutRecalculating(boundingObject);
            CalculateBounds();
        }

        private void AddNegativeWithoutRecalculating(IBoundingObject boundingObject)
        {
            if (_positive == boundingObject || _negatives.Contains(boundingObject))
            {
                return;
            }

            _negatives.Add(boundingObject);
        }

        /// <summary>
        /// Adds multiple <paramref name="boundingObjects"/> to the negative component of the 
        /// <see cref="BoundingObjectGroupDifference"/>. 
        /// Will ignore <see cref="IBoundingObject">IBoundingObjects</see>, that are 
        /// already contained in this <see cref="BoundingObjectGroupUnion"/>.
        /// </summary>
        /// <param name="boundingObjects"></param>
        public void AddNegativeRange(IEnumerable<IBoundingObject> boundingObjects)
        {
            foreach (IBoundingObject boundingObject in boundingObjects)
            {
                AddNegativeWithoutRecalculating(boundingObject);
            }
            CalculateBounds();
        }

        public bool Remove(IBoundingObject boundingObject)
        {
            if (_positive != boundingObject || !_negatives.Contains(boundingObject))
            {
                return false;
            }

            if (_positive == boundingObject)
            {
                _positive = null;
                CalculateBounds();
                return true;
            }

            bool removeEval = _negatives.Remove(boundingObject);

            CalculateBounds();

            return removeEval;
        }

        public void Clear()
        {
            _positive = null;
            _negatives.Clear();
            CalculateBounds();
        }
    }
}
