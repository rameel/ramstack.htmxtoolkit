using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ramstack.HtmxToolkit.Collections;

/// <summary>
/// Provides a compact <see cref="IDictionary{TKey,TValue}" /> implementation
/// optimized for a small number of entries.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
/// <remarks>
/// This type is intended for scenarios in which a dictionary typically contains only a few entries.
/// It uses array-based storage to reduce memory usage and the overhead of creating,
/// populating, and searching the dictionary.
/// <para>
///   For up to <see cref="LinearSearchThreshold" /> entries, keys are located using
///   a linear search. Above this threshold, the entries are sorted by key and
///   subsequent lookups use a binary search. Insertions then preserve the key order.
/// </para>
/// </remarks>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(SmallDictionaryDebugView<,>))]
internal sealed class SmallDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// The maximum number of entries for which the dictionary uses a linear search.
    /// </summary>
    internal const int LinearSearchThreshold = 5;

    private readonly IComparer<TKey> _comparer;
    private KeyValuePair<TKey, TValue>[] _items = [];
    private int _count;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
    public int Count => _count;

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Keys" />
    public KeyCollection Keys => [with(this)];

    /// <inheritdoc cref="IDictionary{TKey,TValue}.Values" />
    public ValueCollection Values => [with(this)];

    /// <inheritdoc cref="IDictionary{TKey,TValue}.this" />
    public TValue this[TKey key]
    {
        get
        {
            ref var item = ref Find(key);
            if (!Unsafe.IsNullRef(ref item))
                return item.Value;

            Error_KeyNotFound();
            return default!;
        }
        set
        {
            var index = IndexOf(key);
            var items = _items;

            if ((uint)index < (uint)items.Length)
            {
                items[index] = new KeyValuePair<TKey, TValue>(items[index].Key, value);
            }
            else
            {
                Insert(~index, key, value);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallDictionary{TKey,TValue}" /> class
    /// using the specified key comparer.
    /// </summary>
    /// <param name="comparer">The comparer to use when comparing keys.</param>
    public SmallDictionary(IComparer<TKey> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        _comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmallDictionary{TKey,TValue}" /> class
    /// with entries copied from the specified collection and the specified
    /// key comparer.
    /// </summary>
    /// <param name="collection">The collection whose entries are copied to the new dictionary.</param>
    /// <param name="comparer">The comparer to use when comparing keys.</param>
    public SmallDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IComparer<TKey> comparer) : this(comparer)
    {
        ArgumentNullException.ThrowIfNull(collection);

        if (collection is Dictionary<TKey, TValue> dictionary)
        {
            _items = new KeyValuePair<TKey, TValue>[dictionary.Count];
            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }
        else if (collection is SmallDictionary<TKey, TValue> small)
        {
            _count = small._count;
            _items = [..small._items];

            if (_count > LinearSearchThreshold && !ReferenceEquals(_comparer, small._comparer))
                Array.Sort(_items, 0, _count, new KeyValuePairComparer(_comparer));
        }
        else
        {
            foreach (var pair in collection)
                Add(pair.Key, pair.Value);
        }
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey" />
    public bool ContainsKey(TKey key) =>
        IndexOf(key) >= 0;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue" />
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref var item = ref Find(key);
        if (!Unsafe.IsNullRef(ref item))
        {
            value = item.Value;
            return true;
        }

        value = default!;
        return false;
    }

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        var index = IndexOf(key);
        if (index >= 0)
        {
            Error_DuplicateKey(key);
            return;
        }

        Insert(~index, key, value);
    }

    /// <summary>
    /// Attempts to add the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>
    /// <see langword="true" /> if the key/value pair was added to the dictionary;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public bool TryAdd(TKey key, TValue value)
    {
        var index = IndexOf(key);
        if (index >= 0)
            return false;

        Insert(~index, key, value);
        return true;
    }

    /// <inheritdoc />
    public bool Remove(TKey key)
    {
        var index = IndexOf(key);
        if (index < 0)
            return false;

        RemoveAt(index);
        return true;
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
        {
            var count = _count;
            if (count != 0)
                Array.Clear(_items, 0, count);
        }

        _count = 0;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="SmallDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Enumerator"/> structure for the <see cref="SmallDictionary{TKey,TValue}"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() =>
        new(this);

    /// <summary>
    /// Gets the underlying array used to store dictionary entries, including any unused capacity.
    /// </summary>
    /// <returns>
    /// The internal backing array. Only the first <see cref="Count" /> elements contain active entries.
    /// </returns>
    internal KeyValuePair<TKey, TValue>[] GetUnderlyingArray() =>
        _items;

    #region ICollection<T>: explicit interface implementations

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    /// <inheritdoc />
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => new KeyCollection(this);

    /// <inheritdoc />
    ICollection<TValue> IDictionary<TKey, TValue>.Values => new ValueCollection(this);

    /// <inheritdoc />
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    /// <inheritdoc />
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    /// <inheritdoc />
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
        Add(item.Key, item.Value);

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        var index = IndexOf(item.Key);
        if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_items[index].Value, item.Value))
            return true;

        return false;
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        _items.AsSpan(0, _count).CopyTo(array.AsSpan(arrayIndex));

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        var index = IndexOf(item.Key);
        var items = _items;

        if ((uint)index >= (uint)items.Length)
            return false;

        if (!EqualityComparer<TValue>.Default.Equals(items[index].Value, item.Value))
            return false;

        RemoveAt(index);
        return true;
    }

    #endregion

    #region IEnumerable<T>: explicit interface implementations

    /// <inheritdoc />
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
        GetEnumerator();

    #endregion

    /// <summary>
    /// Ensures that the dictionary can hold the specified number of entries without resizing.
    /// </summary>
    /// <param name="capacity">The minimum number of entries
    /// that the dictionary must be able to hold.</param>
    private void EnsureCapacity(int capacity)
    {
        const int DefaultCapacity = 4;

        var items = _items;
        var required = items.Length != 0
            ? items.Length * 2
            : DefaultCapacity;

        if ((uint)required < (uint)capacity)
            required = capacity;

        if ((uint)required > Array.MaxLength)
            required = Array.MaxLength;

        var destination = new KeyValuePair<TKey, TValue>[required];
        var count = _count;

        if (count != 0 && (uint)count <= (uint)items.Length)
            items.AsSpan(0, count).CopyTo(destination);

        _items = destination;
    }

    /// <summary>
    /// Inserts the specified key and value at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the entry.</param>
    /// <param name="key">The key of the entry to insert.</param>
    /// <param name="value">The value of the entry to insert.</param>
    private void Insert(int index, TKey key, TValue value)
    {
        if (key == null!)
            Error_NullKey();

        var count = _count;
        var items = _items;

        if (count == items.Length)
        {
            EnsureCapacity(count + 1);
            items = _items;
        }

        if (index < count)
            Array.Copy(items, index, items, index + 1, count - index);

        if ((uint)index < (uint)items.Length)
            items[index] = new KeyValuePair<TKey, TValue>(key, value);

        //
        // Once the entry count exceeds the threshold,
        // the dictionary switches from a linear search over
        // unsorted storage to a binary search over sorted storage.
        // Subsequent insertions preserve key order.
        //
        count++;
        if (count == LinearSearchThreshold + 1)
            Array.Sort(items, 0, count, new KeyValuePairComparer(_comparer));

        _count = count;
    }

    /// <summary>
    /// Finds the entry associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the entry to find.</param>
    /// <returns>
    /// A reference to the entry associated with <paramref name="key" />,
    /// or a <see langword="null"/> reference if the key is not found.
    /// </returns>
    private ref KeyValuePair<TKey, TValue> Find(TKey key)
    {
        var array = _items;
        var count = _count;
        var comparer = _comparer;

        if (count <= LinearSearchThreshold)
        {
            if ((uint)count <= (uint)array.Length)
            {
                var items = array.AsSpan(0, count);
                for (var i = 0; i < items.Length; i++)
                    if (comparer.Compare(items[i].Key, key) == 0)
                        return ref items[i];
            }
        }
        else
        {
            var lo = 0;
            var hi = count - 1;

            while (lo <= hi)
            {
                var mi = (lo + hi) >> 1;
                if ((uint)mi >= (uint)array.Length)
                    break;

                var result = comparer.Compare(array[mi].Key, key);
                if (result == 0)
                    return ref array[mi];

                if (result < 0)
                {
                    lo = mi + 1;
                }
                else
                {
                    hi = mi - 1;
                }
            }
        }

        return ref Unsafe.NullRef<KeyValuePair<TKey, TValue>>();
    }

    /// <summary>
    /// Searches for the specified key and determines where it is or should be located.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>
    /// The zero-based index of <paramref name="key" /> if it is found; otherwise,
    /// the bitwise complement of the index at which the key should be inserted.
    /// </returns>
    private int IndexOf(TKey key)
    {
        var array = _items;
        var count = _count;
        var comparer = _comparer;

        if (count <= LinearSearchThreshold)
        {
            if ((uint)count <= (uint)array.Length)
            {
                var items = array.AsSpan(0, count);
                for (var i = 0; i < items.Length; i++)
                    if (comparer.Compare(items[i].Key, key) == 0)
                        return i;
            }

            return ~count;
        }

        var lo = 0;
        var hi = count - 1;

        while (lo <= hi)
        {
            var mi = lo + (hi - lo >>> 1);
            if ((uint)mi >= (uint)array.Length)
                break;

            var result = comparer.Compare(array[mi].Key, key);
            if (result == 0)
                return mi;

            if (result < 0)
            {
                lo = mi + 1;
            }
            else
            {
                hi = mi - 1;
            }
        }

        return ~lo;
    }

    /// <summary>
    /// Removes the entry at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the entry to remove.</param>
    private void RemoveAt(int index)
    {
        var items = _items;
        var count = _count;
        count--;

        if (index < count)
            Array.Copy(items, index + 1, items, index, count - index);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<KeyValuePair<TKey, TValue>>())
            items[count] = default;

        _count = count;
    }

    /// <summary>
    /// Throws an exception indicating that the requested key was not found.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_KeyNotFound() =>
        throw new KeyNotFoundException();

    /// <summary>
    /// Throws an exception indicating that a dictionary key cannot be <see langword="null" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_NullKey() =>
        throw new ArgumentNullException();

    /// <summary>
    /// Throws an exception indicating that the specified key already exists in the dictionary.
    /// </summary>
    /// <param name="key">The duplicate key.</param>
    /// <exception cref="ArgumentException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_DuplicateKey(TKey key) =>
        throw new ArgumentException($"An item with the same key has already been added. Key: {key}", nameof(key));

    /// <summary>
    /// Throws an exception indicating that the requested operation is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    [DoesNotReturn]
    private static void Error_NotSupported() =>
        throw new NotSupportedException();

    #region Inner type: KeyValuePairComparer

    /// <summary>
    /// Represents a comparer that compares dictionary entries by key.
    /// </summary>
    /// <param name="comparer">The comparer to use when comparing keys.</param>
    private sealed class KeyValuePairComparer(IComparer<TKey> comparer) : IComparer<KeyValuePair<TKey, TValue>>
    {
        /// <inheritdoc />
        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) =>
            comparer.Compare(x.Key, y.Key);
    }

    #endregion

    #region Inner type: KeyCollection

    /// <summary>
    /// Represents the collection of keys in a <see cref="SmallDictionary{TKey,TValue}" />.
    /// </summary>
    /// <param name="dictionary">The dictionary whose keys are exposed by the collection.</param>
    public sealed class KeyCollection(SmallDictionary<TKey, TValue> dictionary) : ICollection<TKey>
    {
        /// <inheritdoc />
        public int Count => dictionary.Count;

        /// <inheritdoc />
        public bool Contains(TKey item) =>
            dictionary.ContainsKey(item);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="KeyCollection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator"/> for the <see cref="KeyCollection"/>.
        /// </returns>
        public Enumerator GetEnumerator() =>
            new(dictionary);

        #region ICollection<T>: explicit interface implementations

        /// <inheritdoc />
        bool ICollection<TKey>.IsReadOnly => true;

        /// <inheritdoc />
        void ICollection<TKey>.Add(TKey item) =>
            Error_NotSupported();

        /// <inheritdoc />
        void ICollection<TKey>.Clear() =>
            Error_NotSupported();

        /// <inheritdoc />
        bool ICollection<TKey>.Remove(TKey item)
        {
            Error_NotSupported();
            return false;
        }

        /// <inheritdoc />
        void ICollection<TKey>.CopyTo(TKey[] array, int arrayIndex)
        {
            var items = dictionary._items.AsSpan(0, dictionary._count);
            var destination = array.AsSpan(arrayIndex);

            for (var index = 0; index < items.Length; index++)
                destination[index] = items[index].Key;
        }

        #endregion

        #region IEnumerable<T>: explicit interface implementations

        /// <inheritdoc />
        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region Inner type: Enumerator

        /// <summary>
        /// Represents an enumerator for the keys in a <see cref="SmallDictionary{TKey,TValue}" />.
        /// </summary>
        public struct Enumerator : IEnumerator<TKey>
        {
            private readonly KeyValuePair<TKey, TValue>[] _items;
            private readonly int _count;
            private int _index;

            /// <inheritdoc />
            public TKey Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _items[_index].Key;
            }

            /// <summary>
            /// Initializes an enumerator for the specified dictionary.
            /// </summary>
            /// <param name="dictionary">The dictionary whose keys are to be enumerated.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(SmallDictionary<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _items = dictionary._items;
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                //
                // The JIT generates a redundant mov instruction for inline ++i (pre-increment).
                // https://github.com/dotnet/runtime/issues/129532
                //

                _index++;
                if (_index < _count && (uint)_index < (uint)_items.Length)
                    return true;

                return false;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }

            #region IEnumerator: Explicit interface implementations

            /// <inheritdoc />
            object System.Collections.IEnumerator.Current => Current!;

            /// <inheritdoc />
            void System.Collections.IEnumerator.Reset() =>
                Error_NotSupported();

            #endregion
        }

        #endregion
    }

    #endregion

    #region Inner type: ValueCollection

    /// <summary>
    /// Represents the collection of values in a <see cref="SmallDictionary{TKey,TValue}" />.
    /// </summary>
    /// <param name="dictionary">The dictionary whose values are exposed by the collection.</param>
    public sealed class ValueCollection(SmallDictionary<TKey, TValue> dictionary) : ICollection<TValue>
    {
        /// <inheritdoc />
        public int Count => dictionary.Count;

        /// <inheritdoc />
        public bool Contains(TValue item)
        {
            var items = dictionary._items.AsSpan(0, dictionary._count);
            foreach (var (_, v) in items)
                if (EqualityComparer<TValue>.Default.Equals(v, item))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ValueCollection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator"/> for the <see cref="ValueCollection"/>.
        /// </returns>
        public Enumerator GetEnumerator() =>
            new(dictionary);

        #region ICollection<T>: explicit interface implementations

        /// <inheritdoc />
        bool ICollection<TValue>.IsReadOnly => true;

        /// <inheritdoc />
        void ICollection<TValue>.Add(TValue item) =>
            Error_NotSupported();

        /// <inheritdoc />
        void ICollection<TValue>.Clear() =>
            Error_NotSupported();

        /// <inheritdoc />
        bool ICollection<TValue>.Remove(TValue item)
        {
            Error_NotSupported();
            return false;
        }

        /// <inheritdoc />
        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            var items = dictionary._items.AsSpan(0, dictionary._count);
            var destination = array.AsSpan(arrayIndex);

            for (var index = 0; index < items.Length; index++)
                destination[index] = items[index].Value;
        }

        #endregion

        #region IEnumerable<T>: explicit interface implementations

        /// <inheritdoc />
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region Inner type: Enumerator

        /// <summary>
        /// Enumerates the values in a <see cref="SmallDictionary{TKey,TValue}" />.
        /// </summary>
        public struct Enumerator : IEnumerator<TValue>
        {
            private readonly KeyValuePair<TKey, TValue>[] _items;
            private readonly int _count;
            private int _index;

            /// <inheritdoc />
            public TValue Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _items[_index].Value;
            }

            /// <summary>
            /// Initializes an enumerator for the specified dictionary.
            /// </summary>
            /// <param name="dictionary">The dictionary whose values are to be enumerated.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(SmallDictionary<TKey, TValue> dictionary)
            {
                _index = -1;
                _count = dictionary._count;
                _items = dictionary._items;
            }

            /// <inheritdoc />
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                //
                // The JIT generates a redundant mov instruction for inline ++i (pre-increment).
                // https://github.com/dotnet/runtime/issues/129532
                //

                _index++;
                if (_index < _count && (uint)_index < (uint)_items.Length)
                    return true;

                return false;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }

            #region IEnumerator: Explicit interface implementations

            /// <inheritdoc />
            object System.Collections.IEnumerator.Current => Current!;

            /// <inheritdoc />
            void System.Collections.IEnumerator.Reset() =>
                Error_NotSupported();

            #endregion
        }

        #endregion
    }

    #endregion

    #region Inner type: Enumerator

    /// <summary>
    /// Represents an enumerator for the entries in a <see cref="SmallDictionary{TKey,TValue}" />.
    /// </summary>
    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly KeyValuePair<TKey, TValue>[] _items;
        private readonly int _count;
        private int _index;

        /// <inheritdoc />
        public KeyValuePair<TKey, TValue> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _items[_index];
        }

        /// <summary>
        /// Initializes an enumerator for the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary whose entries are to be enumerated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(SmallDictionary<TKey, TValue> dictionary)
        {
            _index = -1;
            _count = dictionary._count;
            _items = dictionary._items;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            //
            // The JIT generates a redundant mov instruction for inline ++i (pre-increment).
            // https://github.com/dotnet/runtime/issues/129532
            //

            _index++;
            if (_index < _count && (uint)_index < (uint)_items.Length)
                return true;

            return false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        #region IEnumerator: Explicit interface implementations

        /// <inheritdoc />
        object System.Collections.IEnumerator.Current => Current;

        /// <inheritdoc />
        void System.Collections.IEnumerator.Reset() =>
            Error_NotSupported();

        #endregion
    }

    #endregion
}
