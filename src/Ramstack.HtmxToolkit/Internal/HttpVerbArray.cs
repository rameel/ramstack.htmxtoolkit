using System.Collections;
using System.Runtime.CompilerServices;

namespace Ramstack.HtmxToolkit.Internal;

/// <summary>
/// Represents a lightweight wrapper over an array of <see cref="HttpVerb"/> values.
/// </summary>
internal readonly struct HttpVerbArray : IEnumerable<string>
{
    /// <summary>
    /// Gets the underlying array of <see cref="HttpVerb"/> values.
    /// </summary>
    public HttpVerb[]? Values { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpVerbArray"/> struct.
    /// </summary>
    /// <param name="values">The array of <see cref="HttpVerb"/> values, or <see langword="null" />.</param>
    public HttpVerbArray(HttpVerb[]? values) =>
        Values = values;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() =>
        new(Values);

    /// <inheritdoc />
    IEnumerator<string> IEnumerable<string>.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <summary>
    /// Represents an enumerator that lazily converts <see cref="HttpVerb"/> values
    /// to lowercase string representations.
    /// </summary>
    public struct Enumerator : IEnumerator<string>
    {
        private readonly HttpVerb[] _verbs;
        private int _index;

        /// <inheritdoc />
        public string Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _verbs[_index].GetHttpVerbValue();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> structure.
        /// </summary>
        /// <param name="verbs">The array of <see cref="HttpVerb"/> values, or <see langword="null" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(HttpVerb[]? verbs)
        {
            _verbs = verbs ?? [];
            _index = -1;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            _index++;
            return (uint)_index < (uint)_verbs.Length;
        }

        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Reset() =>
            _index = -1;

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
