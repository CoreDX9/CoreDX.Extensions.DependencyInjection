using System.Collections;

#if NET6_0_OR_GREATER
using System.Collections.Immutable;
#else
using System.Collections.Generic;
#endif


namespace CoreDX.Extensions.DependencyInjection.Proxies;

/// <summary>
/// Record service types that registered as explicit proxy service.
/// </summary>
public interface IOpenGenericServiceProxyRegister :
#if NET6_0_OR_GREATER
    IReadOnlySet<Type>
#else
    ISet<Type>
#endif
    ;

internal interface IStartupOpenGenericServiceProxyRegister : IList<Type>;

/// <summary>
/// Record service types that registered as explicit proxy service.
/// </summary>
/// <param name="types">Registered types.</param>
public sealed class OpenGenericServiceProxyRegister(IEnumerable<Type> types) : IOpenGenericServiceProxyRegister
{
#if NET6_0_OR_GREATER
    private readonly ImmutableHashSet<Type> _types = types.Distinct().ToImmutableHashSet();
#else
    private readonly HashSet<Type> _types = new(types.Distinct());
#endif

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public int Count => _types.Count;

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool Contains(Type item) => _types.Contains(item);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool IsProperSubsetOf(IEnumerable<Type> other) => _types.IsProperSubsetOf(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool IsProperSupersetOf(IEnumerable<Type> other) => _types.IsProperSupersetOf(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool IsSubsetOf(IEnumerable<Type> other) => _types.IsSubsetOf(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool IsSupersetOf(IEnumerable<Type> other) => _types.IsSupersetOf(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool Overlaps(IEnumerable<Type> other) => _types.Overlaps(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public bool SetEquals(IEnumerable<Type> other) => _types.SetEquals(other);

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    public IEnumerator<Type> GetEnumerator() => _types.GetEnumerator();

#if NET6_0_OR_GREATER
    /// <inheritdoc cref="IReadOnlySet{Type}" />
#else
    /// <inheritdoc cref="ISet{Type}" />
#endif
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if NET6_0_OR_GREATER
#else

    /// <inheritdoc cref="ISet{Type}" />
    public bool IsReadOnly => true;

    /// <inheritdoc cref="ISet{Type}" />
    public bool Add(Type item) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void UnionWith(IEnumerable<Type> other) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void IntersectWith(IEnumerable<Type> other) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void ExceptWith(IEnumerable<Type> other) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void SymmetricExceptWith(IEnumerable<Type> other) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    void ICollection<Type>.Add(Type item) => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void Clear() => throw new NotSupportedException();

    /// <inheritdoc cref="ISet{Type}" />
    public void CopyTo(Type[] array, int arrayIndex)
    {
        Array.Copy(this.ToArray(), arrayIndex, array, 0, Count);
    }

    /// <inheritdoc cref="ISet{Type}" />
    public bool Remove(Type item) => throw new NotSupportedException();
#endif
}
