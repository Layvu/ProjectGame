using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectGame;

// Инициализируем его объектами
// Далее если объект изменился вызываем Replace от старого и нового значения
// Если же нам нужны ближайшие объекты к какому-то объекту - вызываем Retrieve
// Вставить новый объект в дерево - Insert
// Остальные методы по большей части служебные
public class QuadTree
{
    private const int MaxObjectsPerNode = 4;
    private const int MaxLevels = 8;

    private readonly int _currentLevel;
    private readonly List<IEntity> _objects;
    private readonly Rectangle _bounds;
    private readonly QuadTree[] _nodes;

    public QuadTree(Rectangle bounds, int level = 0)
    {
        _currentLevel = level;
        _objects = new List<IEntity>();
        _bounds = bounds;
        _nodes = new QuadTree[4];
    }

    public void Insert(IEntity entity)
    {
        if (_nodes[0] != null) // узел имеет дочерние узлы
        {
            var index = GetIndex(entity); // принадлежность к одному из дочерних узлов
            if (index != -1)
            {
                _nodes[index].Insert(entity);
                return;
            }
        }

        _objects.Add(entity);

        if (_objects.Count > MaxObjectsPerNode && _currentLevel < MaxLevels)
        {
            if (_nodes[0] == null) SplitNode();

            var i = 0;
            while (i < _objects.Count)
            {
                var index = GetIndex(_objects[i]); // проверяется на принадлежность к дочерним узлам
                if (index != -1)
                {
                    _nodes[index].Insert(_objects[i]);
                    _objects.RemoveAt(i);
                }
                else i++;
            }
        }
    }

    public List<IEntity> Retrieve(IEntity entity)
    {
        var result = new List<IEntity>();
        var index = GetIndex(entity);
        if (index != -1 && _nodes[0] != null)
            result.AddRange(_nodes[index].Retrieve(entity));
        result.AddRange(_objects);

        return result;
    }

    private void SplitNode()
    {
        var subWidth = _bounds.Width / 2;
        var subHeight = _bounds.Height / 2;
        var x = _bounds.X;
        var y = _bounds.Y;

        _nodes[0] = new QuadTree(new Rectangle(x + subWidth, y, subWidth, subHeight), _currentLevel + 1);
        _nodes[1] = new QuadTree(new Rectangle(x, y, subWidth, subHeight), _currentLevel + 1);
        _nodes[2] = new QuadTree(new Rectangle(x, y + subHeight, subWidth, subHeight), _currentLevel + 1);
        _nodes[3] = new QuadTree(new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight), _currentLevel + 1);
    }

    private int GetIndex(IEntity entity)
    {
        var verticalMidpoint = _bounds.X + _bounds.Width / 2;
        var horizontalMidpoint = _bounds.Y + _bounds.Height / 2;

        var topQuadrant = entity.Position.Y < horizontalMidpoint &&
                          entity.Position.Y + entity.Height < horizontalMidpoint;
        var bottomQuadrant = entity.Position.Y > horizontalMidpoint;

        if (entity.Position.X < verticalMidpoint && entity.Position.X + entity.Width < verticalMidpoint)
        {
            if (topQuadrant) return 1;
            if (bottomQuadrant) return 2;
        }
        else if (entity.Position.X > verticalMidpoint)
        {
            if (topQuadrant) return 0;
            if (bottomQuadrant) return 3;
        }

        return -1;
    }

    public void Update(IEntity entity) //
    {
        var objectsInSameQuad = Retrieve(entity);
        var oldEntity = objectsInSameQuad.FirstOrDefault(e => e.Id == entity.Id);

        if (oldEntity != null)
        {
            Replace(oldEntity, entity);
        }
        else
        {
            Insert(entity);
        }
    }

    public void Replace(IEntity oldEntity, IEntity newEntity) //
    {
        if (!_objects.Contains(oldEntity) && _nodes[0] != null)
        {
            var index = GetIndex(oldEntity);
            if (index != -1)
            {
                _nodes[index].Replace(oldEntity, newEntity);
                return;
            }
        }

        _objects.Remove(oldEntity);
        Insert(newEntity);
    }
}