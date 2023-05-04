#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectGame;

public class QuadTree
{
    private readonly List<ISolid> _elements = new();
    private readonly int _bucketCapacity;
    private readonly int _maxDepth;
    private QuadTree? _topLeft, _topRight, _bottomLeft, _bottomRight;

    public QuadTree(Rectangle bounds) : this(bounds, 4, 8){}

    private QuadTree(Rectangle bounds, int bucketCapacity, int maxDepth)
    {
        _bucketCapacity = bucketCapacity;
        _maxDepth = maxDepth;
     
        Bounds = bounds; // игровая область на экране
    }

    private Rectangle Bounds { get; }

    private int Level { get; set; }

    private bool IsLeaf
        => _topLeft == null || _topRight == null || _bottomLeft == null || _bottomRight == null;

    public void Insert(ISolid element)
    {
        if (element == null || !Bounds.Contains(element.Collider.Boundary))
            throw new ArgumentNullException();

        // узел, превышающий допустимую емкость, будет разбит
        if (_elements.Count >= _bucketCapacity) Split();

        var containingChild = GetContainingChild(element.Collider.Boundary);
        
        if (containingChild == null) _elements.Add(element);
        else containingChild.Insert(element);
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
            var containingChild = GetContainingChild(element.Collider.Boundary);
            if (containingChild != null) // если помещается
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
            Level = Level + 1
        };
    }

    public bool Remove(ISolid element)
    {
        if (element == null) throw new ArgumentNullException();

        var containingChild = GetContainingChild(element.Collider.Boundary);
        
        var removed = containingChild?.Remove(element) ?? _elements.Remove(element);
        if (removed && CountElements() <= _bucketCapacity) Merge();
 
        return removed;
    }
    
    private QuadTree? GetContainingChild(Rectangle bounds) // IShape
    {
        if (IsLeaf) return null;
        
        if (_topLeft.Bounds.Contains(bounds)) return _topLeft;
        if (_topRight.Bounds.Contains(bounds)) return _topRight;
        if (_bottomLeft.Bounds.Contains(bounds)) return _bottomLeft;
        return _bottomRight.Bounds.Contains(bounds) ? _bottomRight : null;
    }

    private int CountElements() 
    {
        var count = _elements.Count;
        if (IsLeaf) return count;
        
        count += _topLeft.CountElements();
        count += _topRight.CountElements();
        count += _bottomLeft.CountElements();
        count += _bottomRight.CountElements();

        return count;
    }
    
    private void Merge()
    {
        if (IsLeaf) return;
 
        _elements.AddRange(_topLeft._elements);
        _elements.AddRange(_topRight._elements);
        _elements.AddRange(_bottomLeft._elements);
        _elements.AddRange(_bottomRight._elements);
 
        _topLeft = _topRight = _bottomLeft = _bottomRight = null;
    }
    
    public IEnumerable<ISolid> FindCollisions(ISolid element)
    {
        if (element == null) throw new ArgumentNullException();
        
        var nodes = new Queue<QuadTree>();
        var collisions = new List<ISolid>();
 
        nodes.Enqueue(this); 
 
        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();
 
            if (!element.Collider.Boundary.Intersects(node.Bounds)) continue;
            
            collisions.AddRange(
                node._elements.Where(e => e.Collider.Boundary.Intersects(element.Collider.Boundary))
                );

            if (node.IsLeaf) continue;
            
            if (element.Collider.Boundary.Intersects(node._topLeft.Bounds))
                nodes.Enqueue(node._topLeft);
            if (element.Collider.Boundary.Intersects(node._topRight.Bounds))
                nodes.Enqueue(node._topRight);
            if (element.Collider.Boundary.Intersects(node._bottomLeft.Bounds))
                nodes.Enqueue(node._bottomLeft);
            if (element.Collider.Boundary.Intersects(node._bottomRight.Bounds))
                nodes.Enqueue(node._bottomRight);
        }
 
        return collisions;
    }
}