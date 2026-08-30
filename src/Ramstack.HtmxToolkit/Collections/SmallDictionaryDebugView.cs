using System.Diagnostics;

namespace Ramstack.HtmxToolkit.Collections;

/// <summary>
/// Provides a debugger view of a <see cref="SmallDictionary{TKey,TValue}" />.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
internal sealed class SmallDictionaryDebugView<TKey, TValue>(SmallDictionary<TKey, TValue>? dictionary) where TKey : notnull
{
    /// <summary>
    /// Gets the active dictionary entries for display in the debugger.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public DictionaryEntry[] Items
    {
        get
        {
            var pairs = dictionary?.ToArray() ?? [];
            var array = new DictionaryEntry[pairs.Length];

            for (var i = 0; i < pairs.Length; i++)
            {
                var (key, value) = pairs[i];
                array[i] = new DictionaryEntry(key, value);
            }

            return array;
        }
    }

    #region Inner type: DictionaryEntry

    /// <summary>
    /// Represents a dictionary entry displayed by the debugger.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <param name="value">The value of the entry.</param>
    [DebuggerDisplay("{Value}", Name = "[{Key}]")]
    public readonly struct DictionaryEntry(TKey key, TValue value)
    {
        /// <summary>
        /// Gets the key of the dictionary entry.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public TKey Key => key;

        /// <summary>
        /// Gets the value of the dictionary entry.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public TValue Value => value;
    }

    #endregion
}
