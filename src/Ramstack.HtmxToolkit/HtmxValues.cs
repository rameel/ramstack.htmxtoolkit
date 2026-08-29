using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents one or more string values for an HTMX request parameter.
/// </summary>
/// <remarks>
/// A single value can be assigned from a string. Multiple values can be specified with a collection expression.
/// </remarks>
[CollectionBuilder(typeof(HtmxValues), nameof(Create))]
[JsonConverter(typeof(HtmxValuesJsonConverter))]
public readonly struct HtmxValues : IReadOnlyList<string>
{
    private readonly object? _values;

    /// <inheritdoc />
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var values = _values;
            if (values is null)
                return 0;

            if (values is string)
                return 1;

            return Unsafe.As<string[]>(values).Length;
        }
    }

    /// <inheritdoc />
    public string this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var values = _values;
            if (values is string s)
            {
                if (index == 0)
                    return s;
            }
            else if (values is not null)
            {
                return Unsafe.As<string[]>(values)[index];
            }

            return OutOfBounds();
        }
    }

    /// <summary>
    /// Gets the underlying value representation.
    /// </summary>
    /// <value>
    /// <see langword="null"/> when the collection is empty, a <see cref="string"/>
    /// when it contains one value, or a string array when it contains multiple values.
    /// </value>
    public object? Values => _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxValues"/> structure with a single value.
    /// </summary>
    /// <param name="value">The value to store.</param>
    public HtmxValues(string value) =>
        _values = value;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxValues"/> structure with the specified values.
    /// </summary>
    /// <param name="values">
    /// The values to store, or <see langword="null"/> to create an empty instance.
    /// </param>
    /// <remarks>
    /// The specified array is stored directly and is not copied.
    /// </remarks>
    public HtmxValues(string[]? values) =>
        _values = values;

    /// <summary>
    /// Creates an <see cref="HtmxValues"/> from the specified values.
    /// </summary>
    /// <param name="values">The values to include.</param>
    /// <returns>
    /// An <see cref="HtmxValues"/> containing the specified values.
    /// </returns>
    public static HtmxValues Create(ReadOnlySpan<string> values)
    {
        return values.Length switch
        {
            0 => new HtmxValues([]),
            1 => new HtmxValues(values[0]),
            _ => new HtmxValues(CreateArray(values))
        };
    }

    /// <summary>
    /// Converts a string to an <see cref="HtmxValues"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator HtmxValues(string value) =>
        new(value);

    /// <summary>
    /// Converts an array of strings to an <see cref="HtmxValues"/>.
    /// </summary>
    /// <param name="values">The values to convert.</param>
    public static implicit operator HtmxValues(string[] values) =>
        new(values);

    /// <summary>
    /// Returns an enumerator that iterates through the values.
    /// </summary>
    /// <returns>
    /// An enumerator for the values.
    /// </returns>
    public Enumerator GetEnumerator() =>
        new(this);

    #region IEnumerable: explicit interface implementations

    /// <inheritdoc />
    IEnumerator<string> IEnumerable<string>.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    #endregion

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string OutOfBounds() =>
        Array.Empty<string>()[0];

    /// <summary>
    /// Copies the specified read-only span to a new array.
    /// </summary>
    /// <param name="s">The values to copy.</param>
    /// <returns>
    /// A new array containing the specified values.
    /// </returns>
    /// <remarks>
    /// The JIT compiler inlines the implementation of <see cref="ReadOnlySpan{T}.ToArray"/>
    /// into its caller, producing a disproportionately large amount of native code at the call site.
    /// This non-inlined wrapper keeps that implementation out of <see cref="Create"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string[] CreateArray(ReadOnlySpan<string> s) =>
        s.ToArray();

    #region Inner type: Enumerator

    /// <summary>
    /// Enumerates the strings represented by an <see cref="HtmxValues"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<string>
    {
        private readonly string[]? _values;
        private string? _current;
        private int _index;

        /// <inheritdoc />
        public readonly string Current => _current!;

        /// <summary>
        /// Initializes a new enumerator for the specified underlying value representation.
        /// </summary>
        /// <param name="value">A single string, an array of strings,
        /// or <see langword="null"/> for an empty sequence.</param>
        private Enumerator(object? value)
        {
            if (value is string s)
            {
                (_values, _current) = (null, s);
            }
            else
            {
                (_values, _current) = (Unsafe.As<string[]?>(value), null);
            }

            _index = 0;
        }

        /// <summary>
        /// Initializes a new enumerator for the specified <see cref="HtmxValues"/>.
        /// </summary>
        /// <param name="value">The value whose strings to enumerate.</param>
        public Enumerator(HtmxValues value) : this(value._values)
        {
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            var index = _index;
            if (index < 0)
                return false;

            var values = _values;
            if (values is not null)
            {
                if ((uint)index < (uint)values.Length)
                {
                    _index = index + 1;
                    _current = values[index];
                    return true;
                }

                _index = -1;
                return false;
            }

            _index = -1;
            return _current is not null;
        }

        /// <inheritdoc />
        public readonly void Dispose()
        {
        }

        #region IEnumerator: explicit interface implementations

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        void IEnumerator.Reset() =>
            throw new NotSupportedException();

        #endregion
    }

    #endregion
}
