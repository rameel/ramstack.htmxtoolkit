namespace Ramstack.HtmxToolkit.Tests;

[TestFixture]
public class HtmxValsTagHelperTests
{
    [Test]
    public async Task ProcessAsync_SerializesValues()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxValsTagHelper();
        helper.Values["category"] = "books";
        helper.Values["sort"] = "title";

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);
        var attribute = output.Attributes["hx-vals"];

        Assert.That(attribute, Is.Not.Null);

        var json = JsonHelper.ParseJson(attribute!.Value.ToString()!);
        Assert.That(json["category"].GetString(), Is.EqualTo("books"));
        Assert.That(json["sort"].GetString(), Is.EqualTo("title"));
    }

    [Test]
    public async Task ProcessAsync_OmitsEmptyDictionary()
    {
        var output = TestHelper.CreateTagHelperOutput();
        var helper = new HtmxValsTagHelper();

        await helper.ProcessAsync(TestHelper.CreateTagHelperContext(), output);

        Assert.That(output.Attributes["hx-vals"], Is.Null);
    }
}
