# Building the documentation locally

From the repository root, restore the tools and build the static site using the same validation target as CI:

```console
dotnet msbuild eng/Validate.proj -t:Docs -tl:off
```

This requires the .NET 10 SDK or later.
Documentation warnings are treated as errors.
The target removes generated API YAML and the previous site before rebuilding, so deleted API types do not
remain in the preview or search index. The authored `docs/api/index.md` is preserved.

Run a local preview server and open <http://localhost:8080>:

```console
dotnet docfx docs/docfx.json --serve
```

DocFX writes generated HTML to `docs/_site` and generated API metadata to `docs/api`;
generated files in both directories are ignored by Git.

## Tutorial examples

Getting started, Forms and validation, Loading states and request errors, MVC, and Minimal APIs contain
runnable examples directly in Markdown.
When changing them, copy the listed files into a fresh application and check the documented browser flow.
Use the current library through a project reference when validating unreleased API changes.

DocFX validates documentation links and references; it does not compile fenced code or the snippets below.
Compile changed C# and Razor examples separately. Check invalid as well as valid form submissions, a second
submission after a swap, and the request triggered by a response event. Include padded short input such as
`" A "` when a setter normalizes values before validation. For request feedback, check a delayed response,
HTTP 500, a connection failure, a timeout, and a successful retry. Check the JavaScript payload listener too
when changing event examples.

## API examples

API examples are maintained outside the library source code. Markdown overwrite files in
`docs/api-overwrites` attach an `example` to an API page by its DocFX `uid`, while the
referenced source snippets live in `docs/snippets`.

To add an example, find the generated UID in `docs/api/*.yml`, add an overwrite section,
and reference a snippet with DocFX's `[!code-csharp[](...)]` or `[!code-razor[](...)]` syntax.
