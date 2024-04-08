using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PositionEvents.Area;
using PositionEvents.Implementation.OcTree.Bounds;

namespace PositionEvents.Implementation.OcTree
{
    public class Node
    {
        private BoundingBox _bounds;

        private readonly List<IBoundingObject> _objects;
        private Node[] _children;
        private Node[,,] _childrenByPosition;

        private readonly OcTree _tree;

        /// <summary>
        /// The bounds of this <see cref="Node"/>.
        /// </summary>
        public BoundingBox Bounds => _bounds;

        /// <summary>
        /// Whether the <see cref="Node"/> has children.
        /// </summary>
        public bool HasChildren => _children.Length > 0;

        /// <summary>
        /// The amount of objects in this <see cref="Node"/> and it's child <see cref="Node">Nodes</see>.
        /// </summary>
        public int Count
        {
            get
            {
                int count = _objects.Count();
                foreach (Node child in _children)
                {
                    count += child.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Whether the <see cref="Node"/> or it's children have any <see cref="IBoundingObject">objects</see>.
        /// </summary>
        public bool HasObjects
        {
            get
            {
                if (_objects.Any())
                {
                    return true;
                }

                if (_children.Any(child => child.HasObjects))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// True, if at least one of the child <see cref="Node">Nodes</see> has children.
        /// </summary>
        public bool IsDeep
        {
            get
            {
                return _children.Any(child => child.HasChildren);
            }
        }

        public Node(BoundingBox bounds, OcTree tree)
        {
            _bounds = bounds;
            _objects = new List<IBoundingObject>();
            _children = Array.Empty<Node>();
            _childrenByPosition = new Node[0,0,0];
            _tree = tree;
        }

        /// <summary>
        /// Returns all <see cref="IBoundingObject">IBoundingObjects</see> of the <see cref="Node"/> and it's 
        /// children.
        /// </summary>
        /// <returns>All contained <see cref="IBoundingObject">IBoundingObjects</see>.</returns>
        public IBoundingObject[] Flatten()
        {
            List<IBoundingObject> result = new List<IBoundingObject>();

            result.AddRange(_objects);
            foreach(Node child in _children)
            {
                result.AddRange(child.Flatten());
            }

            return result.ToArray();
        }

        /// <summary>
        /// Adds a new <see cref="IBoundingObject"/> to this <see cref="Node"/>. Will try to add the given 
        /// <paramref name="boundingObject"/> to a child <see cref="Node"/>, if possible.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the given <paramref name="boundingObject"/> fits inside the <see cref="_bounds"/> 
        /// of this <see cref="Node"/>. Otherwise false.</returns>
        public bool Add(IBoundingObject boundingObject)
        {
            if (!_bounds.Contains(boundingObject))
            {
                return false;
            }

            foreach (Node child in _children)
            {
                if (child.Add(boundingObject))
                {
                    return true;
                }
            }

            _objects.Add(boundingObject);

            if (!HasChildren && _objects.Count() > _tree.MaxObjectsPerNode)
            {
                Split();
            }

            return true;
        }

        /// <summary>
        /// Adds the new <see cref="IBoundingObject">IBoundingObjects</see> to this <see cref="Node"/>. 
        /// Will try to add the given 
        /// <paramref name="boundingObjects"/> to a child <see cref="Node"/>, if possible.
        /// </summary>
        /// <param name="boundingObjects"></param>
        /// <returns>True, if all given <paramref name="boundingObjects"/> fit inside the <see cref="_bounds"/> 
        /// of this <see cref="Node"/>. Otherwise false.</returns>
        public bool AddRange(IEnumerable<IBoundingObject> boundingObjects)
        {
            return boundingObjects.All(boundingObject => Add(boundingObject));
        }

        /// <summary>
        /// Removes the given <paramref name="boundingObject"/> from this <see cref="Node"/> and merges 
        /// with it's child <see cref="Node">Nodes</see> if neccessary.
        /// </summary>
        /// <param name="boundingObject"></param>
        /// <returns>True, if the <paramref name="boundingObject"/> is part of this <see cref="Node"/> 
        /// or it's children.</returns>
        public bool Remove(IBoundingObject boundingObject)
        {
            if (!_bounds.Contains(boundingObject))
            {
                return false;
            }

            bool removed = _children.Any(child => child.Remove(boundingObject));

            removed = removed || _objects.Remove(boundingObject);

            if (removed && ShouldMerge())
            {
                Merge();
            }

            return removed;
        }

        /// <summary>
        /// Removes all <see cref="IBoundingObject">IBoundingObjects</see> from the <see cref="Node"/>. Also removes 
        /// all the <see cref="IBoundingObject">IBoundingObjects</see> from it's children and the child 
        /// <see cref="Node">Nodes</see> itself.
        /// </summary>
        public void RemoveAll()
        {
            _objects.Clear();
            foreach (Node child in _children)
            {
                child.RemoveAll();
            }
            _children = Array.Empty<Node>();
        }

        /// <summary>
        /// Checks every relevant <see cref="IBoundingObject"/> in this <see cref="Node"/> and it's 
        /// relevant children, to see if 
        /// the given <paramref name="position"/> is contained in it.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>An Array of <see cref="IBoundingObject">IBoundingObjects</see>, that contain the given 
        /// <paramref name="position"/>.</returns>
        public IBoundingObject[] GetContaining(Vector3 position)
        {
            if (_bounds.Contains(position) == ContainmentType.Disjoint)
            {
                return Array.Empty<IBoundingObject>();
            }
            
            List<IBoundingObject> result = new List<IBoundingObject>();

            result.AddRange(_objects.Where(boundingObject => boundingObject.ContainsEfficent(position)));

            foreach(Node child in GetAppropriateChildNodes(position))
            {
                result.AddRange(child.GetContaining(position));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Calculates the child <see cref="Node">Nodes</see>, that the given <paramref name="position"/> would 
        /// fit into.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>The child <see cref="Node">Nodes</see>, that the given <paramref name="position"/> 
        /// fits into.</returns>
        private Node[] GetAppropriateChildNodes(Vector3 position)
        {
            if (!HasChildren)
            {
                return Array.Empty<Node>();
            }

            int[][] childPositions = _bounds.GetChildPositions(position);

            if (childPositions.Length == 0)
            {
                return Array.Empty<Node>();
            }

            List<Node> result = new List<Node>();

            foreach (int[] childPosition in childPositions)
            {
                result.Add(_childrenByPosition[childPosition[0], childPosition[1], childPosition[2]]);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Splits the <see cref="Node"/> into 8 children. And refits the current 
        /// <see cref="_objects"/> into the new children.
        /// </summary>
        private void Split()
        {
            if (HasChildren)
            {
                throw new InvalidOperationException("Can't split Node, that already has children.");
            }

            List<Node> children = new List<Node>();

            _childrenByPosition = new Node[2, 2, 2];

            foreach (BoundingBox subBox in _bounds.GetSubBoxes())
            {
                Node newNode = new Node(subBox, _tree);
                _objects.RemoveAll(obj => newNode.Add(obj));

                children.Add(newNode);

                int[] childPosition = _bounds.GetChildPositions(subBox.GetCenter()).First();
                _childrenByPosition[childPosition[0], childPosition[1], childPosition[2]] = newNode;
            }

            _children = children.ToArray();
        }

        /// <summary>
        /// Removes the <see cref="Node">Node's</see> <see cref="_children"/> and adds their 
        /// <see cref="IBoundingObject">IBoundingObjects</see> to this <see cref="_objects"/>.
        /// </summary>
        private void Merge()
        {
            foreach(Node child in _children)
            {
                _objects.AddRange(child._objects);
                child._objects.Clear();
            }

            _children = Array.Empty<Node>();
            _childrenByPosition = new Node[0, 0, 0];
        }

        /// <summary>
        /// Determines whether the <see cref="Node"/> should merge it's child <see cref="Node">Nodes</see> into 
        /// itself.
        /// </summary>
        /// <returns>True, if the <see cref="Node"/> is not deep and <see cref="Count"/> is less or 
        /// equal to the <see cref="OcTree">tree's</see> MaxObjectsPerNode.</returns>
        private bool ShouldMerge()
        {
            if (IsDeep)
            {
                return false;
            }

            return Count <= _tree.MaxObjectsPerNode;
        }
    }
}
