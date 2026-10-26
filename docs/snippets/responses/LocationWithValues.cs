// After archiving, reload the product list with its filters applied.
Response.Htmx(htmx =>
    htmx.Location(
        Url.Action("Index", "Products")!,
        new HtmxLocationOptions
        {
            Target = "#product-list",
            Values = new Dictionary<string, HtmxFieldValues>
            {
                ["status"] = "archived",          // One value.
                ["category"] = ["books", "music"] // Several values of one field.
            }
        }));
