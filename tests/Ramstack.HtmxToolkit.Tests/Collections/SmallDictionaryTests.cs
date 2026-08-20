using Ramstack.HtmxToolkit.Collections;

namespace Ramstack.HtmxToolkit.Tests.Collections;

[TestFixture]
public class SmallDictionaryTests
{
    [Test]
    public void Constructor_NewInstance_IsEmpty_Writable()
    {
        var dictionary = CreateDictionary();
        var collection = AsCollection(dictionary);

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.Zero);
            Assert.That(dictionary.Keys, Is.Empty);
            Assert.That(dictionary.Values, Is.Empty);
            Assert.That(collection.IsReadOnly, Is.False);
            Assert.That(collection, Is.Empty);
        });
    }

    [Test]
    public void Add_NewKey_StoresEntry_UpdatesAllViews()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[7], Is.EqualTo("seven"));
            Assert.That(dictionary.ContainsKey(7), Is.True);
            Assert.That(dictionary.Keys, Is.EquivalentTo([7]));
            Assert.That(dictionary.Values, Is.EquivalentTo(["seven"]));
        });
    }

    [Test]
    public void Add_DuplicateKey_ThrowsArgumentException_PreservesExistingEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "original");

        Assert.Throws<ArgumentException>(() => dictionary.Add(7, "replacement"));

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[7], Is.EqualTo("original"));
        });
    }

    [Test]
    public void Add_NullKey_ThrowsArgumentNullException_PreservesState()
    {
        IDictionary<string, int> dictionary = new SmallDictionary<string, int>(StringComparer.Ordinal);
        dictionary.Add("existing", 1);

        Assert.Throws<ArgumentNullException>(() => dictionary.Add(null!, 0));

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary["existing"], Is.EqualTo(1));
        });
    }

    [Test]
    public void Indexer_Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var dictionary = CreateDictionary();

        Assert.Throws<KeyNotFoundException>(() => _ = dictionary[42]);
    }

    [Test]
    public void Indexer_Set_MissingKey_AddsEntry()
    {
        var dictionary = CreateDictionary();
        dictionary[7] = "first";

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[7], Is.EqualTo("first"));
            Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(7, "first")]));
        });
    }

    [Test]
    public void Indexer_Set_ExistingKey_UpdatesEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "first");

        dictionary[7] = "updated";

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[7], Is.EqualTo("updated"));
            Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(7, "updated")]));
        });
    }

    [Test]
    public void Indexer_Set_NullKey_ThrowsArgumentNullException_PreservesState()
    {
        IDictionary<string, int> dictionary = new SmallDictionary<string, int>(StringComparer.Ordinal);
        dictionary.Add("existing", 1);

        Assert.Throws<ArgumentNullException>(() => dictionary[null!] = 0);

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary["existing"], Is.EqualTo(1));
        });
    }

    [Test]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");

        Assert.That(dictionary.ContainsKey(7), Is.True);
    }

    [Test]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");

        Assert.That(dictionary.ContainsKey(42), Is.False);
    }

    [Test]
    public void TryGetValue_ExistingKey_ReturnsTrue()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");

        var found = dictionary.TryGetValue(7, out var existingValue);

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.True);
            Assert.That(existingValue, Is.EqualTo("seven"));
        });
    }

    [Test]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");

        var missing = dictionary.TryGetValue(42, out var missingValue);

        Assert.Multiple(() =>
        {
            Assert.That(missing, Is.False);
            Assert.That(missingValue, Is.Null);
        });
    }

    [Test]
    public void Remove_ByKey_ExistingKey_ReturnsTrue_RemovesEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");

        var removed = dictionary.Remove(1);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.ContainsKey(1), Is.False);
            Assert.That(dictionary[2], Is.EqualTo("two"));
        });
    }

    [Test]
    public void Remove_ByKey_MissingKey_ReturnsFalse_PreservesState()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(2, "two");

        var removed = dictionary.Remove(1);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.False);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[2], Is.EqualTo("two"));
        });
    }

    [Test]
    public void Remove_LinearSearchMode_ClearsUnusedReferenceSlots()
    {
        var dictionaryWithReferenceKeys = new SmallDictionary<string, int>(StringComparer.Ordinal);
        var dictionaryWithReferenceValues = new SmallDictionary<int, string>(Comparer<int>.Default);

        for (var index = 0; index < SmallDictionary<int, string>.LinearSearchThreshold; index++)
        {
            dictionaryWithReferenceKeys.Add($"key-{index}", index);
            dictionaryWithReferenceValues.Add(index, $"value-{index}");
        }

        Assert.That(dictionaryWithReferenceKeys.Remove("key-2"), Is.True);
        Assert.That(dictionaryWithReferenceValues.Remove(2), Is.True);

        AssertUnusedSlotsAreCleared(dictionaryWithReferenceKeys);
        AssertUnusedSlotsAreCleared(dictionaryWithReferenceValues);
    }

    [Test]
    public void Remove_AfterLinearSearchThreshold_ClearsUnusedReferenceSlots()
    {
        var dictionaryWithReferenceKeys = new SmallDictionary<string, int>(StringComparer.Ordinal);
        var dictionaryWithReferenceValues = new SmallDictionary<int, string>(Comparer<int>.Default);

        for (var index = 0; index < SmallDictionary<int, string>.LinearSearchThreshold + 2; index++)
        {
            dictionaryWithReferenceKeys.Add($"key-{index}", index);
            dictionaryWithReferenceValues.Add(index, $"value-{index}");
        }

        Assert.That(dictionaryWithReferenceKeys.Remove("key-3"), Is.True);
        Assert.That(dictionaryWithReferenceValues.Remove(3), Is.True);

        AssertUnusedSlotsAreCleared(dictionaryWithReferenceKeys);
        AssertUnusedSlotsAreCleared(dictionaryWithReferenceValues);
    }

    [Test]
    public void Clear_PopulatedDictionary_RemovesAllEntries_AllowsReuse()
    {
        var dictionary = CreateDictionary();
        for (var key = 0; key < 50; key++)
            dictionary.Add(key, $"value-{key}");

        dictionary.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.Zero);
            Assert.That(dictionary.Keys, Is.Empty);
            Assert.That(dictionary.Values, Is.Empty);
            Assert.That(dictionary.ContainsKey(10), Is.False);
        });

        dictionary.Add(100, "reused");

        Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(100, "reused")]));
    }

    [Test]
    public void Collection_Add_KeyValuePair_AddsEntry()
    {
        var dictionary = CreateDictionary();
        var collection = AsCollection(dictionary);
        var pair = KeyValuePair.Create(7, "seven");

        collection.Add(pair);

        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary[7], Is.EqualTo("seven"));
        });
    }

    [Test]
    public void Collection_Contains_KeyValuePair_RequiresMatchingKeyAndValue()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(7, "seven");
        var collection = AsCollection(dictionary);

        Assert.Multiple(() =>
        {
            Assert.That(collection.Contains(KeyValuePair.Create(7, "seven")), Is.True);
            Assert.That(collection.Contains(KeyValuePair.Create(7, "different")), Is.False);
            Assert.That(collection.Contains(KeyValuePair.Create(42, "seven")), Is.False);
        });
    }

    [Test]
    public void Collection_Remove_MatchingKeyAndValue_ReturnsTrue_RemovesEntry()
    {
        var dictionary = CreateDictionary();
        var collection = AsCollection(dictionary);

        dictionary.Add(1, "one");
        dictionary.Add(2, "two");

        var removed = collection.Remove(KeyValuePair.Create(1, "one"));

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(2, "two")]));
        });
    }

    [Test]
    public void Collection_Remove_MismatchedValue_ReturnsFalse_PreservesEntry()
    {
        var dictionary = CreateDictionary();
        var collection = AsCollection(dictionary);

        dictionary.Add(1, "one");

        var removed = collection.Remove(KeyValuePair.Create(1, "different"));

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.False);
            Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(1, "one")]));
        });
    }

    [Test]
    public void Collection_Remove_MissingKey_ReturnsFalse_PreservesState()
    {
        var dictionary = CreateDictionary();
        var collection = AsCollection(dictionary);

        dictionary.Add(1, "one");

        var removed = collection.Remove(KeyValuePair.Create(3, "three"));

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.False);
            Assert.That(dictionary, Is.EquivalentTo([KeyValuePair.Create(1, "one")]));
        });
    }

    [Test]
    public void Collection_CopyTo_WithOffset_CopiesEveryEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(3, "three");
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");
        var destination = new KeyValuePair<int, string>[5];

        AsCollection(dictionary).CopyTo(destination, 1);

        Assert.Multiple(() =>
        {
            Assert.That(destination[0], Is.EqualTo(default(KeyValuePair<int, string>)));
            Assert.That(destination[1..4], Is.EquivalentTo(dictionary));
            Assert.That(destination[4], Is.EqualTo(default(KeyValuePair<int, string>)));
        });
    }

    [Test]
    public void Keys_DictionaryChanges_AreReflectedInLiveCollection()
    {
        var dictionary = CreateDictionary();
        var keys = dictionary.Keys;

        dictionary.Add(3, "three");
        dictionary.Add(1, "one");

        Assert.Multiple(() =>
        {
            Assert.That(keys.Count, Is.EqualTo(2));
            Assert.That(keys.Contains(1), Is.True);
            Assert.That(keys.Contains(2), Is.False);
            Assert.That(keys, Is.EquivalentTo([1, 3]));
        });
    }

    [Test]
    public void Keys_CopyTo_WithOffset_CopiesAllKeys()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(3, "three");
        dictionary.Add(1, "one");
        var destination = new[] { -1, -1, -1, -1 };

        dictionary.Keys.CopyTo(destination, 1);

        Assert.Multiple(() =>
        {
            Assert.That(destination[0], Is.EqualTo(-1));
            Assert.That(destination[1..3], Is.EquivalentTo([1, 3]));
            Assert.That(destination[3], Is.EqualTo(-1));
        });
    }

    [Test]
    public void Keys_Mutation_ThrowsNotSupportedException()
    {
        var keys = CreateDictionary().Keys;

        Assert.That(keys.IsReadOnly, Is.True);
        AssertReadOnlyCollection(keys, 5);
    }

    [Test]
    public void Values_DictionaryChanges_AreReflectedInLiveCollectionIncludingDuplicates()
    {
        var dictionary = CreateDictionary();
        var values = dictionary.Values;
        dictionary.Add(3, "same");
        dictionary.Add(1, "same");
        dictionary.Add(2, "other");

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values.Contains("same"), Is.True);
            Assert.That(values.Contains("missing"), Is.False);
            Assert.That(values, Is.EquivalentTo(["same", "same", "other"]));
        });
    }

    [Test]
    public void Values_CopyTo_WithOffset_CopiesAllValuesIncludingDuplicates()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(3, "same");
        dictionary.Add(1, "same");
        dictionary.Add(2, "other");
        var destination = new[] { "sentinel", "sentinel", "sentinel", "sentinel", "sentinel" };

        dictionary.Values.CopyTo(destination, 1);

        Assert.Multiple(() =>
        {
            Assert.That(destination[0], Is.EqualTo("sentinel"));
            Assert.That(destination[1..4], Is.EquivalentTo(["same", "same", "other"]));
            Assert.That(destination[4], Is.EqualTo("sentinel"));
        });
    }

    [Test]
    public void Values_Mutation_ThrowsNotSupportedException()
    {
        var values = CreateDictionary().Values;

        Assert.That(values.IsReadOnly, Is.True);
        AssertReadOnlyCollection(values, "new");
    }

    [Test]
    public void Enumeration_Generic_ReturnsEveryEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(3, "three");
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");

        var expected = new[]
        {
            KeyValuePair.Create(1, "one"),
            KeyValuePair.Create(2, "two"),
            KeyValuePair.Create(3, "three")
        };

        Assert.That(dictionary, Is.EquivalentTo(expected));
    }

    [Test]
    public void Enumeration_NonGeneric_ReturnsEveryEntry()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(3, "three");
        dictionary.Add(1, "one");
        dictionary.Add(2, "two");

        var expected = new[]
        {
            KeyValuePair.Create(1, "one"),
            KeyValuePair.Create(2, "two"),
            KeyValuePair.Create(3, "three")
        };

        var entries = dictionary.ToArray();
        Assert.That(entries, Is.EquivalentTo(expected));
    }

    [Test]
    public void Enumerators_Reset_ThrowsNotSupportedException()
    {
        var dictionary = CreateDictionary();
        dictionary.Add(1, "one");

        Assert.Multiple(() =>
        {
            Assert.Throws<NotSupportedException>(dictionary.GetEnumerator().Reset);
            Assert.Throws<NotSupportedException>(dictionary.Keys.GetEnumerator().Reset);
            Assert.Throws<NotSupportedException>(dictionary.Values.GetEnumerator().Reset);
        });
    }

    [Test]
    public void ConfiguredComparer_AllOperations_UseComparerForKeyIdentity()
    {
        IDictionary<string, int> dictionary = new SmallDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary.Add("Alpha", 1);

        var found = dictionary.TryGetValue("ALPHA", out var value);
        dictionary["alpha"] = 2;

        Assert.Multiple(() =>
        {
            Assert.That(found, Is.True);
            Assert.That(value, Is.EqualTo(1));
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.ContainsKey("aLpHa"), Is.True);
            Assert.That(dictionary["ALPHA"], Is.EqualTo(2));
            Assert.That(dictionary.Keys.Contains("alpha"), Is.True);
            Assert.That(AsCollection(dictionary).Contains(KeyValuePair.Create("ALPHA", 2)), Is.True);
            Assert.That(() => dictionary.Add("ALPHA", 3), Throws.ArgumentException);
        });

        Assert.That(dictionary.Remove("aLpHa"), Is.True);
        Assert.That(dictionary, Is.Empty);
    }

    [Test]
    public void DictionaryOperations_AtLinearSearchThreshold_RemainCorrect()
    {
        var dictionary = CreateDictionary();
        var expected = new Dictionary<int, string>();

        for (var key = SmallDictionary<int, string>.LinearSearchThreshold - 1; key >= 0; key--)
        {
            var value = $"value-{key}";
            dictionary.Add(key, value);
            expected.Add(key, value);
        }

        AssertDictionaryState(dictionary, expected);
        Assert.Multiple(() =>
        {
            Assert.That(dictionary.ContainsKey(-1), Is.False);
            Assert.That(dictionary.TryGetValue(100, out _), Is.False);
            Assert.That(() => _ = dictionary[100], Throws.TypeOf<KeyNotFoundException>());
        });

        dictionary[2] = "updated";
        expected[2] = "updated";

        Assert.That(dictionary.Remove(0), Is.True);
        expected.Remove(0);

        AssertDictionaryState(dictionary, expected);
    }

    [Test]
    public void Add_ExceedingLinearSearchThreshold_PreservesEntriesDuringSearchTransition()
    {
        var dictionary = CreateDictionary();
        var expected = new Dictionary<int, string>();
        var threshold = SmallDictionary<int, string>.LinearSearchThreshold;

        for (var index = 0; index < threshold; index++)
        {
            var key = (threshold - index) * 10;
            dictionary.Add(key, $"value-{key}");
            expected.Add(key, $"value-{key}");
        }

        var transitionKey = 25;
        dictionary.Add(transitionKey, $"value-{transitionKey}");
        expected.Add(transitionKey, $"value-{transitionKey}");

        Assert.That(dictionary.Count, Is.EqualTo(threshold + 1));
        AssertDictionaryState(dictionary, expected);
    }

    [Test]
    public void DictionaryOperations_AfterSearchTransition_RemainCorrect()
    {
        var dictionary = CreateDictionary();
        var expected = new Dictionary<int, string>();
        var initialKeys = new[] { 50, 10, 40, 20, 30, 25 };

        foreach (var key in initialKeys)
        {
            dictionary.Add(key, $"value-{key}");
            expected.Add(key, $"value-{key}");
        }

        foreach (var key in new[] { -10, 15, 35, 60 })
        {
            dictionary.Add(key, $"value-{key}");
            expected.Add(key, $"value-{key}");
        }

        dictionary[30] = "updated";
        expected[30] = "updated";
        Assert.That(dictionary.Remove(10), Is.True);

        expected.Remove(10);
        Assert.That(AsCollection(dictionary).Remove(KeyValuePair.Create(35, "value-35")), Is.True);

        expected.Remove(35);
        AssertDictionaryState(dictionary, expected);
    }

    [Test]
    public void DictionaryOperations_AfterFallingBelowThreshold_RemainCorrectThroughSecondTransition()
    {
        var (dictionary, expected) = CreateDictionaryAfterFallingBelowThreshold();
        var threshold = SmallDictionary<int, string>.LinearSearchThreshold;

        Assert.That(dictionary.Count, Is.EqualTo(threshold - 2));
        Assert.That(dictionary.Count, Is.GreaterThan(1));
        AssertDictionaryState(dictionary, expected);

        foreach (var key in new[] { 55, 15 })
        {
            dictionary.Add(key, $"value-{key}");
            expected.Add(key, $"value-{key}");
            AssertDictionaryState(dictionary, expected);
        }

        Assert.That(dictionary.Count, Is.EqualTo(threshold));

        dictionary.Add(35, "value-35");
        expected.Add(35, "value-35");

        Assert.That(dictionary.Count, Is.EqualTo(threshold + 1));
        AssertDictionaryState(dictionary, expected);
    }

    [Test]
    public void Add_DuplicateKeyAfterFallingBelowThreshold_ThrowsArgumentException_PreservesState()
    {
        var (dictionary, expected) = CreateDictionaryAfterFallingBelowThreshold();
        dictionary.Add(55, "value-55");
        expected.Add(55, "value-55");

        Assert.That(
            dictionary.Count,
            Is.GreaterThan(1).And.LessThan(SmallDictionary<int, string>.LinearSearchThreshold));

        Assert.Throws<ArgumentException>(() => dictionary.Add(55, "duplicate"));
        Assert.Throws<ArgumentException>(() => dictionary.Add(40, "duplicate"));

        AssertDictionaryState(dictionary, expected);
    }

    [Test]
    public void Resize_GrowingToOneHundredEntries_PreservesStateAcrossRepeatedResizes()
    {
        const int EntryCount = 100;

        var dictionary = CreateDictionary();
        var expected = new Dictionary<int, string>();

        for (var index = 0; index < EntryCount; index++)
        {
            var key = index * 37 % EntryCount;
            var value = $"value-{key}";

            dictionary.Add(key, value);
            expected.Add(key, value);
        }

        AssertDictionaryState(dictionary, expected);

        for (var key = 0; key < EntryCount; key += 3)
        {
            dictionary[key] = $"updated-{key}";
            expected[key] = $"updated-{key}";
        }

        for (var key = 0; key < EntryCount; key += 4)
        {
            Assert.That(dictionary.Remove(key), Is.True);
            expected.Remove(key);
        }

        AssertDictionaryState(dictionary, expected);
    }

    private static IDictionary<int, string> CreateDictionary() =>
        new SmallDictionary<int, string>(Comparer<int>.Default);

    private static (IDictionary<int, string> Dictionary, Dictionary<int, string> Expected) CreateDictionaryAfterFallingBelowThreshold()
    {
        var dictionary = CreateDictionary();
        var expected = new Dictionary<int, string>();
        var threshold = SmallDictionary<int, string>.LinearSearchThreshold;

        for (var index = 1; index <= threshold + 2; index++)
        {
            var key = index * 10;
            dictionary.Add(key, $"value-{key}");
            expected.Add(key, $"value-{key}");
        }

        for (var key = 10; dictionary.Count > threshold - 2; key += 20)
        {
            dictionary.Remove(key);
            expected.Remove(key);
        }

        return (dictionary, expected);
    }

    private static ICollection<KeyValuePair<TKey, TValue>> AsCollection<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : notnull =>
        dictionary;

    private static void AssertUnusedSlotsAreCleared<TKey, TValue>(SmallDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        var items = dictionary.GetUnderlyingArray();
        for (var index = dictionary.Count; index < items.Length; index++)
        {
            Assert.That(
                items[index],
                Is.EqualTo(default(KeyValuePair<TKey, TValue>)),
                $"Backing array slot {index} must be cleared when Count is {dictionary.Count}.");
        }
    }

    private static void AssertReadOnlyCollection<T>(ICollection<T> collection, T value)
    {
        Assert.Multiple(() =>
        {
            Assert.That(() => collection.Add(value), Throws.TypeOf<NotSupportedException>());
            Assert.That(() => collection.Remove(value), Throws.TypeOf<NotSupportedException>());
            Assert.That(collection.Clear, Throws.TypeOf<NotSupportedException>());
        });
    }

    private static void AssertDictionaryState(IDictionary<int, string> actual, IReadOnlyDictionary<int, string> expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
            Assert.That(actual, Is.EquivalentTo(expected));
            Assert.That(actual.Keys, Is.EquivalentTo(expected.Keys));
            Assert.That(actual.Values, Is.EquivalentTo(expected.Values));
        });

        foreach (var (key, expectedValue) in expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.ContainsKey(key), Is.True, $"Key {key} must be present.");
                Assert.That(actual.TryGetValue(key, out var actualValue), Is.True, $"Key {key} must be found.");
                Assert.That(actualValue, Is.EqualTo(expectedValue), $"TryGetValue returned a wrong value for key {key}.");
                Assert.That(actual[key], Is.EqualTo(expectedValue), $"Indexer returned a wrong value for key {key}.");
            });
        }
    }
}
