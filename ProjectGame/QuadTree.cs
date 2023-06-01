using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class QuadTree
{
    public QuadTree(Rectangle bounds) : this(bounds, 4, 8){}
    private QuadTree(Rectangle bounds, int bucketCapacity, int maxDepth)
    {
        _bucketCapacity = bucketCapacity;
        _maxDepth = maxDepth;
        
        Bounds = bounds;
    }
    
    private readonly List<ISolid> _elements = new();
    private readonly int _bucketCapacity;
    private readonly int _maxDepth;
    private QuadTree _topLeft, _topRight, _bottomLeft, _bottomRight;
    private int _elementCount; 
    private QuadTree _parent;

    private Rectangle Bounds { get; }
    private int Level { get; set; }
    private bool IsLeaf
        => _topLeft == null || _topRight == null || _bottomLeft == null || _bottomRight == null;

    public void Insert(ISolid element)
    {
        if (Contains(element)) return;

        if (_elements.Count >= _bucketCapacity) Split();

        var containingChild = GetContainingChild(element);

        if (containingChild == null) _elements.Add(element);
        else containingChild.Insert(element);

        _elementCount++;
    }
    
    public bool Remove(ISolid element)
    {
        bool wasRemoved;

        var containingChild = GetContainingChild(element);
        if (containingChild != null) wasRemoved = containingChild.Remove(element);
        else wasRemoved = _elements.Remove(element);
        
        if (wasRemoved && _elementCount <= _bucketCapacity) Merge();
        if (wasRemoved) _elementCount--;

        return wasRemoved;
    }

    private bool Contains(ISolid element)
    {
        if (_elements.Contains(element)) return true;

        if (!IsLeaf)
        {
            return _topLeft.Contains(element) 
                   || _topRight.Contains(element)
                   || _bottomLeft.Contains(element) 
                   || _bottomRight.Contains(element);
        }
        return false;
    }


    private void Split()
    {  
        if (!IsLeaf || Level + 1 > _maxDepth) return;
 
        _topLeft = CreateChild(Bounds.Location);
        _topRight = CreateChild(new Point(Bounds.Center.X, Bounds.Location.Y));
        _bottomLeft = CreateChild(new Point(Bounds.Location.X, Bounds.Center.Y));
        _bottomRight = CreateChild(new Point(Bounds.Center.X, Bounds.Center.Y));
 
        var elements = _elements.ToList();
        foreach (var element in elements)
        {
            var containingChild = GetContainingChild(element);
            if (containingChild != null) 
            {   
                _elements.Remove(element);
                containingChild.Insert(element);
            }
        }
    }

    private QuadTree CreateChild(Point location)
    {
        return new QuadTree(
            new Rectangle(location, new Point(Bounds.Size.X / 2, Bounds.Size.Y / 2)), 
            _bucketCapacity, _maxDepth)
        {
            Level = Level + 1,
            _parent = this
        };
    }

    private QuadTree GetContainingChild(ISolid element)
    {
        if (IsLeaf) return null;
        
        if (_topLeft.Bounds.Contains(element.Collider.Boundary)) return _topLeft;
        if (_topRight.Bounds.Contains(element.Collider.Boundary)) return _topRight;
        if (_bottomLeft.Bounds.Contains(element.Collider.Boundary)) return _bottomLeft;
        return _bottomRight.Bounds.Contains(element.Collider.Boundary) ? _bottomRight : null;
    }

    private void Merge()
    {
        if (IsLeaf) return;

        _elements.AddRange(_topLeft._elements);
        _elements.AddRange(_topRight._elements);
        _elements.AddRange(_bottomLeft._elements);
        _elements.AddRange(_bottomRight._elements);

        _topLeft = _topRight = _bottomLeft = _bottomRight = null;
        
        var parent = _parent;
        while (parent != null)
        {
            parent._elementCount = parent._elements.Count;
            parent = parent._parent;
        }
    }

    public IEnumerable<ISolid> FindNearbyObjects(ISolid element)
    {
        if (element == null) throw new ArgumentNullException();
        
        var stackToCheck = new Stack<QuadTree>();
        var result = new HashSet<ISolid>();

        stackToCheck.Push(this);
        var elementBounds = element.Collider.Boundary;
        while (stackToCheck.Count > 0)
        {
            var node = stackToCheck.Pop();
            if (!elementBounds.Intersects(node.Bounds)) continue;
            
            result.UnionWith(node._elements.Except(new[] { element }));
            
            if (!node.IsLeaf)
            {
                if (elementBounds.Intersects(node._topLeft.Bounds))
                    stackToCheck.Push(node._topLeft);
                if (elementBounds.Intersects(node._topRight.Bounds))
                    stackToCheck.Push(node._topRight);
                if (elementBounds.Intersects(node._bottomLeft.Bounds))
                    stackToCheck.Push(node._bottomLeft);
                if (elementBounds.Intersects(node._bottomRight.Bounds))
                    stackToCheck.Push(node._bottomRight);
            }
        }

        return result;
    }
}