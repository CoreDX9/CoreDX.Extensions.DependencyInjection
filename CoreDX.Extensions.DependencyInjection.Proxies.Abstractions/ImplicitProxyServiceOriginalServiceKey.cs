using System.Numerics;

namespace CoreDX.Extensions.DependencyInjection.Proxies;

/// <summary>
/// Service key for access original service that already added as implicit proxy.
/// </summary>
public sealed class ImplicitProxyServiceOriginalServiceKey
    : IEquatable<ImplicitProxyServiceOriginalServiceKey>
#if NET7_0_OR_GREATER
    , IEqualityOperators<ImplicitProxyServiceOriginalServiceKey, ImplicitProxyServiceOriginalServiceKey, bool>
    , IEqualityOperators<ImplicitProxyServiceOriginalServiceKey, object, bool>
#endif
{
    private const int _hashCodeBase = 870983858;

    private readonly bool _isStringMode;
    private readonly object? _originalServiceKey;

    private static readonly ImplicitProxyServiceOriginalServiceKey _default = CreateOriginalServiceKey(null);
    private static readonly ImplicitProxyServiceOriginalServiceKey _stringDefault = CreateStringOriginalServiceKey(null);

    /// <summary>
    /// Prefix for access original <see cref="string"/> based keyed service that already added as implicit proxy.
    /// </summary>
    public const string DefaultStringPrefix = $"[{nameof(CoreDX)}.{nameof(Extensions)}.{nameof(DependencyInjection)}.{nameof(Proxies)}.{nameof(ImplicitProxyServiceOriginalServiceKey)}](ImplicitDefault)";

    /// <summary>
    /// Default original service key for none keyed proxy service.
    /// </summary>
    public static ImplicitProxyServiceOriginalServiceKey Default => _default;

    /// <summary>
    /// Default original service key for none <see cref="string"/> based keyed proxy service.
    /// </summary>
    public static ImplicitProxyServiceOriginalServiceKey StringDefault => _stringDefault;

    /// <summary>
    /// Service key of original service.
    /// </summary>
    public object? OriginalServiceKey => _originalServiceKey;

    public bool Equals(ImplicitProxyServiceOriginalServiceKey? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        if (_isStringMode && obj is string str) return $"{DefaultStringPrefix}{_originalServiceKey}" == str;
        else
        {
            var isEquals = obj is not null and ImplicitProxyServiceOriginalServiceKey other
            && ((_originalServiceKey is null && other._originalServiceKey is null) || _originalServiceKey?.Equals(other._originalServiceKey) is true);

            return isEquals;
        }
    }

    public static bool operator ==(ImplicitProxyServiceOriginalServiceKey? left, ImplicitProxyServiceOriginalServiceKey? right)
    {
        return left?.Equals(right) is true;
    }

    public static bool operator !=(ImplicitProxyServiceOriginalServiceKey? left, ImplicitProxyServiceOriginalServiceKey? right)
    {
        return !(left == right);
    }

    public static bool operator ==(ImplicitProxyServiceOriginalServiceKey? left, object? right)
    {
        return left?.Equals(right) is true;
    }

    public static bool operator !=(ImplicitProxyServiceOriginalServiceKey? left, object? right)
    {
        return !(left == right);
    }

    public static bool operator ==(object? left, ImplicitProxyServiceOriginalServiceKey? right)
    {
        return right == left;
    }

    public static bool operator !=(object? left, ImplicitProxyServiceOriginalServiceKey? right)
    {
        return right != left;
    }

    public override int GetHashCode()
    {
        return _isStringMode
            ? $"{DefaultStringPrefix}{_originalServiceKey}".GetHashCode()
            : HashCode.Combine(_hashCodeBase, _originalServiceKey);
    }

    /// <summary>
    /// Creates an instance of <see cref="ImplicitProxyServiceOriginalServiceKey"/> with the specified service key in <paramref name="originalServiceKey"/>.
    /// </summary>
    /// <param name="originalServiceKey"></param>
    /// <returns>A new instance of <see cref="ImplicitProxyServiceOriginalServiceKey"/>.</returns>
    public static ImplicitProxyServiceOriginalServiceKey CreateOriginalServiceKey(object? originalServiceKey)
    {
        return new(originalServiceKey, false);
    }

    /// <summary>
    /// Creates an instance of <see cref="ImplicitProxyServiceOriginalServiceKey"/> with the specified <see cref="string"/> based service key in <paramref name="originalServiceKey"/>.
    /// </summary>
    /// <param name="originalServiceKey"></param>
    /// <returns>A new instance of <see cref="ImplicitProxyServiceOriginalServiceKey"/>.</returns>
    public static ImplicitProxyServiceOriginalServiceKey CreateStringOriginalServiceKey(string? originalServiceKey)
    {
        return new(originalServiceKey, true);
    }

    private ImplicitProxyServiceOriginalServiceKey(object? originalServiceKey, bool isStringMode)
    {
        _originalServiceKey = originalServiceKey;
        _isStringMode = isStringMode;
    }
}