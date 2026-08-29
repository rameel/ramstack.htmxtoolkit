using System.Collections;
using System.Text.Json;

namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxFieldValuesTests
{
    [Test]
    public void DefaultValue_IsEmpty()
    {
        var value = default(HtmxFieldValues);

        Assert.Multiple(() =>
        {
            Assert.That(value.Count, Is.Zero);
            Assert.That(value.Values, Is.Null);
            Assert.That(value, Is.Empty);
            Assert.That(value, Is.EquivalentTo(Array.Empty<string>()));
        });
    }

    [Test]
    public void Enumeration_DefaultValue_ForeachCompletesWithoutItems()
    {
        var values = default(HtmxFieldValues);
        var list = new List<string>();

        Assert.DoesNotThrow(() =>
        {
            foreach (var item in values)
                list.Add(item);
        });

        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Constructor_SingleValue_UsesStringRepresentation()
    {
        var values = new HtmxFieldValues("value");
        Assert.That(values, Is.EqualTo(["value"]));
    }

    [Test]
    public void Constructor_Array_StoresArrayWithoutCopying()
    {
        var source = new[] { "first", "second" };
        var values = new HtmxFieldValues(source);

        source[1] = "changed";

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.EqualTo(2));
            Assert.That(values.Values, Is.SameAs(source));
            Assert.That(values[1], Is.EqualTo("changed"));
            Assert.That(values, Is.EqualTo(source));
        });
    }

    [Test]
    public void Constructor_EmptyArray_PreservesArrayRepresentation()
    {
        var source = new string [0];
        var values = new HtmxFieldValues(source);

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.Zero);
            Assert.That(values.Values, Is.SameAs(source));
            Assert.That(values, Is.Empty);
        });
    }

    [Test]
    public void Constructor_NullArray_IsEmpty()
    {
        var values = new HtmxFieldValues((string[]?)null);

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.Zero);
            Assert.That(values.Values, Is.Null);
            Assert.That(values, Is.Empty);
        });
    }

    [Test]
    public void Create_EmptySpan_ReturnsEmptyArrayRepresentation()
    {
        var values = HtmxFieldValues.Create(ReadOnlySpan<string>.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.Zero);
            Assert.That(values.Values, Is.TypeOf<string[]>());
            Assert.That(values.Values, Is.Empty);
        });
    }

    [Test]
    public void Create_SingleValue_UsesStringRepresentation()
    {
        var source = new[] { "single" };
        var values = HtmxFieldValues.Create(source);

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.EqualTo(1));
            Assert.That(values.Values, Is.TypeOf<string>());
            Assert.That(values[0], Is.EqualTo("single"));
            Assert.That(values, Is.EqualTo(["single"]));
        });
    }

    [Test]
    public void Create_MultipleValues_CopiesTheSpan()
    {
        var source = new[] { "before", "first", "second", "after" };
        var values = HtmxFieldValues.Create(source.AsSpan());
        source[1] = "changed";

        Assert.Multiple(() =>
        {
            Assert.That(values.Count, Is.EqualTo(4));
            Assert.That(values.Values, Is.TypeOf<string[]>());
            Assert.That(values.Values, Is.Not.SameAs(source));
            Assert.That(values, Is.EqualTo(["before", "first", "second", "after"]));
        });
    }

    [Test]
    public void CollectionExpressions_SelectCompactRepresentations()
    {
        HtmxFieldValues empty = [];
        HtmxFieldValues single = ["one"];
        HtmxFieldValues multiple = ["one", "two"];

        Assert.Multiple(() =>
        {
            Assert.That(empty.Values, Is.TypeOf<string[]>());
            Assert.That(empty, Is.Empty);
            Assert.That(single.Values, Is.TypeOf<string>());
            Assert.That(single, Is.EqualTo(["one"]));
            Assert.That(multiple.Values, Is.TypeOf<string[]>());
            Assert.That(multiple, Is.EqualTo(["one", "two"]));
        });
    }

    [Test]
    public void ImplicitConversions_PreserveSourceRepresentations()
    {
        var array = new[] { "one", "two" };

        HtmxFieldValues single = "value";
        HtmxFieldValues multiple = array;

        Assert.Multiple(() =>
        {
            Assert.That(single.Values, Is.EqualTo("value"));
            Assert.That(single, Is.EqualTo(["value"]));
            Assert.That(multiple.Values, Is.SameAs(array));
            Assert.That(multiple, Is.EqualTo(array));
        });
    }

    [TestCase(-1)]
    [TestCase(1)]
    public void Indexer_SingleValueInvalidIndex_ThrowsException(int index)
    {
        var values = new HtmxFieldValues("value");

        Assert.Throws<IndexOutOfRangeException>(() => _ = values[index]);
    }

    [TestCase(-1)]
    [TestCase(2)]
    public void Indexer_ArrayInvalidIndex_ThrowsException(int index)
    {
        var values = new HtmxFieldValues(["one", "two"]);

        Assert.Throws<IndexOutOfRangeException>(() => _ = values[index]);
    }

    [Test]
    public void Enumeration_GenericAndNonGenericInterfaces_ReturnValuesInOrder()
    {
        HtmxFieldValues values = ["first", "second", "third"];

        // ReSharper disable once RedundantCast
        var generic = ((IEnumerable<string>)values).ToArray();

        // ReSharper disable once RedundantCast
        var nonGeneric = ((IEnumerable)values).Cast<string>().ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(generic, Is.EqualTo(["first", "second", "third"]));
            Assert.That(nonGeneric, Is.EqualTo(generic));
        });
    }

    [Test]
    public void Enumerator_AfterSequenceEnds_RemainsCompleted()
    {
        var enumerator = new HtmxFieldValues.Enumerator(["first", "second"]);

        Assert.Multiple(() =>
        {
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo("first"));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo("second"));
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        });
    }

    [Test]
    public void Enumerator_Reset_ThrowsNotSupportedException()
    {
        IEnumerator enumerator = new HtmxFieldValues.Enumerator("value");

        Assert.Throws<NotSupportedException>(enumerator.Reset);
    }

    [Test]
    public void JsonSerialization_EmptySingleAndMultipleValues_UsesExpectedShapes()
    {
        HtmxFieldValues single = "a\"b";
        HtmxFieldValues multiple = ["first", "second"];

        Assert.Multiple(() =>
        {
            Assert.That(JsonSerializer.Serialize(default(HtmxFieldValues)), Is.EqualTo("[]"));
            Assert.That(JsonSerializer.Serialize(single), Is.EqualTo("\"a\\u0022b\""));
            Assert.That(JsonSerializer.Serialize(multiple), Is.EqualTo("[\"first\",\"second\"]"));
        });
    }
}
