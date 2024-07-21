using System.Collections.Concurrent;

namespace codecrafters_http_server.Utils;

public class ObjectPool<T> where T : class
{
    private readonly int _maxSize;
    private readonly ConcurrentBag<T> _heap = new();
    private readonly Func<T> _factory;
    private readonly Action<T> _releaser;

    private int _instanceCount;
    private readonly object _lock = new();

    public ObjectPool(Func<T> factory, Action<T> releaser, int maxSize)
    {
        _factory = factory;
        _releaser = releaser;
        _maxSize = maxSize;
    }
    
    public RentedObject<T> Rent()
    {
        var instance = GetInstance();
        return new RentedObject<T>(instance, this);
    }

    private T GetInstance()
    {
        if (_heap.TryTake(out var result))
        {
            return result;
        }

        return AllocateNewInstance();
    }

    private T AllocateNewInstance()
    {
        lock (_lock)
        {
            if (_instanceCount >= _maxSize)
            {
                throw new InvalidOperationException($"Pool is full. Max size: {_maxSize}");
            }

            var instance = _factory();
            _releaser(instance);
            _instanceCount++;
            //We don't add the object to the heap, since we're immediately renting it
            return instance;
        }
    }
    
    public void Return(T obj)
    {
        _releaser(obj);
        _heap.Add(obj);
    }
}

public readonly struct RentedObject<T> : IDisposable
where T : class
{
    private readonly ObjectPool<T> _pool;
    public T Value { get; }
    
    public RentedObject(T value, ObjectPool<T> pool)
    {
        Value = value;
        _pool = pool;
    }
    
    public void Dispose()
    {
        _pool.Return(Value);
    }
    
    public static implicit operator T(RentedObject<T> obj) => obj.Value;
}