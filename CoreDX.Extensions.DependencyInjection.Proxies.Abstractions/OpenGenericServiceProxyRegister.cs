using System.Collections;
using System.Collections.Immutable;

namespace CoreDX.Extensions.DependencyInjection.Proxies;

/// <summary>
/// Record service types that registered as explicit proxy service.
/// </summary>
public interface IOpenGenericServiceProxyRegister : IReadOnlySet<Type>;

internal interface IStartupOpenGenericServiceProxyRegister : IList<Type>;

/// <summary>
/// Record service types that registered as explicit proxy service.
/// </summary>
/// <param name="types">Registered types.</param>
public sealed class OpenGenericServiceProxyRegister(IEnumerable<Type> types) : IOpenGenericServiceProxyRegister
{
    private readonly ImmutableHashSet<Type> _types = types.Distinct().ToImmutableHashSet();

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public int Count => _types.Count;

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool Contains(Type item) => _types.Contains(item);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool IsProperSubsetOf(IEnumerable<Type> other) => _types.IsProperSubsetOf(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool IsProperSupersetOf(IEnumerable<Type> other) => _types.IsProperSupersetOf(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool IsSubsetOf(IEnumerable<Type> other) => _types.IsSubsetOf(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool IsSupersetOf(IEnumerable<Type> other) => _types.IsSupersetOf(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool Overlaps(IEnumerable<Type> other) => _types.Overlaps(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public bool SetEquals(IEnumerable<Type> other) => _types.SetEquals(other);

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    public IEnumerator<Type> GetEnumerator() => _types.GetEnumerator();

    /// <inheritdoc cref="IReadOnlySet{Type}" />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
