using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents one or more values for a form field submitted with an HTMX request.
/// </summary>
[JsonConverter(typeof(HtmxFieldValuesJsonConverter))]
[CollectionBuilder(typeof(HtmxFieldValues), nameof(Create))]
public readonly struct HtmxFieldValues : IReadOnlyList<string>
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
    /// Gets the underlying storage: a string, a string array, or <see langword="null" />.
    /// </summary>
    internal object? Values => _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxFieldValues" /> structure
    /// with a single value.
    /// </summary>
    /// <param name="value">The value to store, or <see langword="null" />
    /// to represent no values.</param>
    public HtmxFieldValues(string? value) =>
        _values = value;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmxFieldValues" /> structure
    /// with the specified values.
    /// </summary>
    /// <remarks>The specified array is stored directly and is not copied.</remarks>
    /// <param name="values">The values to store, or <see langword="null" />
    /// to represent no values.</param>
    public HtmxFieldValues(string[]? values) =>
        _values = values;

    /// <summary>
    /// Creates an <see cref="HtmxFieldValues" /> from the specified values.
    /// </summary>
    /// <param name="values">The values to include.</param>
    /// <returns>
    /// An <see cref="HtmxFieldValues" /> containing the specified values.
    /// </returns>
    public static HtmxFieldValues Create(ReadOnlySpan<string> values)
    {
        return values.Length switch
        {
            0 => new HtmxFieldValues([]),
            1 => new HtmxFieldValues(values[0]),
            _ => new HtmxFieldValues(CreateArray(values))
        };
    }

    /// <summary>
    /// Converts a string to an <see cref="HtmxFieldValues" />.
    /// </summary>
    /// <param name="value">The value to convert, or <see langword="null" />
    /// to represent no values.</param>
    /// <returns>
    /// An <see cref="HtmxFieldValues" /> containing <paramref name="value" />.
    /// </returns>
    public static implicit operator HtmxFieldValues(string? value) =>
        new(value);

    /// <summary>
    /// Converts an array of strings to an <see cref="HtmxFieldValues" />.
    /// </summary>
    /// <param name="values">The values to convert, or <see langword="null" />
    /// to represent no values.</param>
    /// <returns>
    /// An <see cref="HtmxFieldValues" /> containing <paramref name="values" />.
    /// </returns>
    public static implicit operator HtmxFieldValues(string[]? values) =>
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

    /// <summary>
    /// Throws the standard exception produced by indexing an empty array.
    /// </summary>
    /// <returns>
    /// This method does not return.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException">Always thrown.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string OutOfBounds() =>
        Array.Empty<string>()[0];

    /// <summary>
    /// Copies the specified read-only span to a new array.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The JIT compiler inlines the implementation of <see cref="ReadOnlySpan{T}.ToArray" />
    ///   into its caller, producing a disproportionately large amount of native code
    ///   at the call site.
    /// </para>
    /// <para>
    ///   This non-inlined wrapper keeps that implementation out of <see cref="Create" />.
    /// </para>
    /// </remarks>
    /// <param name="s">The values to copy.</param>
    /// <returns>
    /// A new array containing the specified values.
    /// </returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string[] CreateArray(ReadOnlySpan<string> s) =>
        s.ToArray();

    #region Inner type: Enumerator

    /// <summary>
    /// Enumerates the strings represented by an <see cref="HtmxFieldValues" />.
    /// </summary>
    public struct Enumerator : IEnumerator<string>
    {
        private readonly string[]? _values;
        private string? _current;
        private int _index;

        /// <inheritdoc />
        public readonly string Current => _current!;

        /// <summary>
        /// Initializes a new enumerator for the specified <see cref="HtmxFieldValues" />.
        /// </summary>
        /// <param name="value">The value whose strings to enumerate.</param>
        public Enumerator(HtmxFieldValues value)
        {
            if (value.Values is string s)
            {
                (_values, _current) = (null, s);
            }
            else
            {
                (_values, _current) = (Unsafe.As<string[]?>(value.Values), null);
            }

            _index = 0;
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
