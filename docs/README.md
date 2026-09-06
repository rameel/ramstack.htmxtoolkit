# Building the documentation locally

Restore the repository-local DocFX version:

```console
dotnet tool restore
```

Build the static site:

```console
dotnet docfx docs/docfx.json
```

Run a local preview server and open <http://localhost:8080>:

```console
dotnet docfx docs/docfx.json --serve
```

DocFX writes generated HTML to `docs/_site` and generated API metadata to `docs/api`;
both directories are ignored by Git.

## API examples

API examples are maintained outside the library source code. Markdown overwrite files in
`docs/api-overwrites` attach an `example` to an API page by its DocFX `uid`, while the
referenced source snippets live in `docs/snippets`.

To add an example, find the generated UID in `docs/api/*.yml`, add an overwrite section,
and reference a snippet with DocFX's `[!code-csharp[](...)]` or `[!code-razor[](...)]` syntax.
