using System.Diagnostics;

namespace Ramstack.HtmxToolkit.Collections;

/// <summary>
/// Represents a debugger view for the <see cref="SmallDictionary{TKey,TValue}"/> class, allowing inspection of its contents.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
internal sealed class SmallDictionaryDebugView<TKey, TValue>(SmallDictionary<TKey, TValue>? dictionary) where TKey : notnull
{
    /// <summary>
    /// Gets the array of key-value pairs contained in the <see cref="SmallDictionary{TKey,TValue}"/> for debugging purposes.
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
    /// Represents a class that contains the key/value pairs of the dictionary entry for displaying by a debugger.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <param name="key">The value of the entry.</param>
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
