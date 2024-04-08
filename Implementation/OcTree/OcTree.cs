using Microsoft.Xna.Framework;
using PositionEvents.Area;

namespace PositionEvents.Implementation.OcTree
{
    public class OcTree
    {
        private readonly int _maxObjectsPerNode;

        private Node _root;

        public int MaxObjectsPerNode => _maxObjectsPerNode;

        public BoundingBox Bounds => _root.Bounds;

        public OcTree(int maxObjectsPerNode, BoundingBox bounds)
        {
            _maxObjectsPerNode = maxObjectsPerNode;

            _root = new Node(bounds, this);
        }

        /// <summary>
        /// Adds a new <see cref="IBoundingObject"/> to this <see cref="OcTree"/>.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the given <paramref name="boundingObject"/> fits inside the <see cref="Bounds"/> 
        /// of this <see cref="OcTree"/>. Otherwise false.</returns>
        public bool Add(IBoundingObject boundingObject)
        {
            return _root.Add(boundingObject);
        }

        /// <summary>
        /// Removes the given <paramref name="boundingObject"/> from this <see cref="OcTree"/>.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the <paramref name="boundingObject"/> is part of this <see cref="OcTree"/>.</returns>
        public bool Remove(IBoundingObject boundingObject)
        {
            return _root.Remove(boundingObject);
        }

        /// <summary>
        /// Checks every <see cref="IBoundingObject"/> in this <see cref="OcTree"/>, to see if 
        /// the given <paramref name="position"/> is contained in it.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>An Array of <see cref="IBoundingObject">IBoundingObjects</see>, that contain the given 
        /// <paramref name="position"/>.</returns>
        public IBoundingObject[] GetContaining(Vector3 position)
        {
            return _root.GetContaining(position);
        }

        /// <summary>
        /// Creates new <see cref="Node">Nodes</see> to fit the given <paramref name="newBounds"/>.
        /// </summary>
        /// <param name="newBounds"></param>
        public void Resize(BoundingBox newBounds)
        {
            IBoundingObject[] boundingObjects = _root.Flatten();
            _root.RemoveAll();

            _root = new Node(newBounds, this);

            _root.AddRange(boundingObjects);

            // TODO: do something with the overhang.
        }

        /// <summary>
        /// Resizes the <see cref="OcTree"/> to include the given <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds"></param>
        public void ResizeToInclude(BoundingBox bounds)
        {
            if (_root.Bounds.Contains(bounds) == ContainmentType.Contains)
            {
                return;
            }

            BoundingBox newBounds = BoundingBox.CreateMerged(_root.Bounds, bounds);

            Resize(newBounds);
        }

        /// <summary>
        /// Returns all <see cref="IBoundingObject">IBoundingObjects</see> of the <see cref="OcTree"/>.
        /// </summary>
        /// <returns>All contained <see cref="IBoundingObject">IBoundingObjects</see>.</returns>
        public IBoundingObject[] Flatten()
        {
            return _root.Flatten();
        }

        /// <summary>
        /// Removes all <see cref="IBoundingObject">IBoundingObjects</see> from the <see cref="OcTree"/>.
        /// </summary>
        public void Clear()
        {
            _root.RemoveAll();
        }
    }
}
