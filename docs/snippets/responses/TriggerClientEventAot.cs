var detail = new ProductSaved(product.Id);

Response.Htmx(
    static (htmx, detail) => htmx.TriggerEvent(
        "product-saved",
        detail,
        AppJsonContext.Default.ProductSaved,
        HtmxTriggerTiming.AfterSwap),
    detail);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ProductSaved))]
internal partial class AppJsonContext : JsonSerializerContext;

internal sealed record ProductSaved(int Id);
