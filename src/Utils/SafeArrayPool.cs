using System.Buffers;

namespace codecrafters_http_server.Utils;

public class SafeArrayPool<T>
{
    private readonly ArrayPool<T> _arrayPool;
    
    public SafeArrayPool(int maxArrayLength, int maxArraysPerBucket)
    {
        _arrayPool = ArrayPool<T>.Create(maxArrayLength, maxArraysPerBucket);
    }
    
    public RentedArray<T> Rent(int length)
    {
        var array = _arrayPool.Rent(length);
        return new RentedArray<T>(array, this);
    }
    
    public void Return(T[] array)
    {
        _arrayPool.Return(array);
    }
}

public readonly struct RentedArray<T> : IDisposable
{
    private readonly SafeArrayPool<T> _safeArrayPool;
    public T[] Array { get; }

    public RentedArray(T[] array, SafeArrayPool<T> safeArrayPool)
    {
        Array = array;
        _safeArrayPool = safeArrayPool;
    }
    
    public static implicit operator T[](RentedArray<T> rentedArray) => rentedArray.Array;

    public void Dispose()
    {
        _safeArrayPool.Return(Array);
    }
}