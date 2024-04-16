using System.Collections;
using System.Collections.Immutable;

namespace CoreDX.Extensions.DependencyInjection.Proxies;

public interface IOpenGenericServiceProxyRegister : IReadOnlySet<Type>;

internal interface IStartupOpenGenericServiceProxyRegister : IList<Type>;

public sealed class OpenGenericServiceProxyRegister(IEnumerable<Type> types) : IOpenGenericServiceProxyRegister
{
    private readonly ImmutableHashSet<Type> _types = types.Distinct().ToImmutableHashSet();

    public int Count => _types.Count;

    public bool Contains(Type item) => _types.Contains(item);

    public bool IsProperSubsetOf(IEnumerable<Type> other) => _types.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<Type> other) => _types.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<Type> other) => _types.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<Type> other) => _types.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<Type> other) => _types.Overlaps(other);

    public bool SetEquals(IEnumerable<Type> other) => _types.SetEquals(other);

    public IEnumerator<Type> GetEnumerator() => _types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
