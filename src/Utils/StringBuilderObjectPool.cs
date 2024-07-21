using System.Text;

namespace codecrafters_http_server.Utils;

public class StringBuilderObjectPool : ObjectPool<StringBuilder>
{
    public StringBuilderObjectPool(int maxSize, int defaultCapacity)
        : base(() => new StringBuilder(defaultCapacity), static sb => sb.Clear(), maxSize)
    {
    }
}