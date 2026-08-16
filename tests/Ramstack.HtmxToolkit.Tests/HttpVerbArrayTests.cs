namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HttpVerbArrayTests
{
    [Test]
    public void EnumeratesVerbs_AsLowercaseStrings()
    {
        var verbs = new HttpVerbArray([HttpVerb.Get, HttpVerb.Post, HttpVerb.Delete]);
        Assert.That(verbs, Is.EqualTo(new[] { "get", "post", "delete" }));
    }

    [Test]
    public void EnumeratesEmpty_ForNullArray()
    {
        var verbs = new HttpVerbArray(null);
        Assert.That(verbs, Is.Empty);
    }

    [Test]
    public void EnumeratesEmpty_ForEmptyArray()
    {
        var verbs = new HttpVerbArray([]);
        Assert.That(verbs, Is.Empty);
    }

    [Test]
    public void Values_ExposesUnderlyingArray()
    {
        var array = new[] { HttpVerb.Put };
        var verbs = new HttpVerbArray(array);

        Assert.That(verbs.Values, Is.SameAs(array));
    }
}
