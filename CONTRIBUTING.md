# Contributing

Bug reports, fixes, and documentation improvements are welcome. Small contributions are welcome too.

## Reporting bugs

Tell us what happened and what you expected. Include the HtmxToolkit, .NET, and HTMX versions you're using.
A small example that reproduces the problem helps.

For browser issues, include your browser version and any relevant console errors.

## Development setup

Install the .NET 10 SDK or later to build from source.

Run the commands below from the repository root.

## Building and testing

Build the solution and run the tests:

```console
dotnet test
```

Compiler and analyzer warnings are treated as errors.

## Code style

Follow [.editorconfig](.editorconfig) and the style of the surrounding code.
Keep formatting changes limited to the code you're working on.

## Working on scripts and documentation

### Companion script

Install Node.js and pnpm when working on the companion script.

Edit `src/Ramstack.HtmxToolkit/wwwroot/htmx-toolkit.js`, then rebuild the script assets:

```console
pnpm install --frozen-lockfile
pnpm run build
```

Include the updated `htmx-toolkit.js` and `htmx-toolkit.min.js` in the same commit.
Use the [demo application](README.md#demo) to check browser-side behavior with the affected HTMX versions.

### Documentation

Guides live in `docs/articles`. API examples are maintained in `docs/api-overwrites` and `docs/snippets`.
See [Building the documentation locally](docs/README.md) for build commands and instructions for adding API examples.

## Additional checks

The optional scripts in [`eng/`](eng/) run additional checks.
Choose `Test`, `Aot`, `JavaScript`, or `Docs`, or use `All` to run them together.

- Linux and macOS: `bash eng/validate.sh <target>`
- Windows: `.\eng\validate.cmd <target>`

### Native AOT validation

The AOT check validates the built NuGet package. It requires the platform's
[Native AOT prerequisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot/#prerequisites).

```console
dotnet msbuild eng/Validate.proj -t:Aot -tl:off
```

## Pull requests

- Keep each pull request focused on one change.
- Explain what changed and why, and link to related issues if there are any.
- When fixing a reproducible bug, add a regression test. Add tests for new behavior where appropriate.
- Update the documentation if your change affects how people use the library.
- Keep changes compatible with .NET 6 and the supported HTMX versions.
  See [Version compatibility](docs/articles/version-compatibility.md).

For major public API changes, open an issue first so we can agree on the approach before you spend time on it.

Use the [scoped commit style](#commit-messages) for pull request titles. For a pull request that spans multiple scopes,
choose the scope that best matches its main change. The title does not need to summarize every supporting commit.

## Commit messages

Use scoped commit messages:

```text
<scope>: <concise description>
```

The scope identifies the part of HtmxToolkit that changed. Write the description in the imperative mood.
Describe what the change does rather than just naming its type.

```text
http: detect boosted navigation
tag-helpers: render HTMX 4 configuration modifiers
config: remove the HTMX 4 MetaCharacter option
```

Do not prefix messages with conventional commit types such as `feat`, `fix`, `refactor`, or `chore`.
The description should make the type of change clear when it matters.

### Scopes

Use the most specific scope that represents a stable part of the project. Start with the root scopes below.
Add a subscope only when the commit history for a root scope becomes difficult to navigate.

| Scope         | Area                                                                      |
|---------------|---------------------------------------------------------------------------|
| `http`        | HTTP request detection, headers, MVC selection, and response APIs         |
| `tag-helpers` | Razor Tag Helpers                                                         |
| `config`      | Application, version-specific, and antiforgery configuration              |
| `hosting`     | Endpoint mapping, dependency registration, and server-side asset delivery |
| `script`      | Browser-side companion script behavior                                    |
| `internals`   | Shared implementation code that does not belong to a single product area  |
| `docs`        | Documentation covering multiple product areas and DocFX tooling           |
| `demo`        | Demo-only application structure and presentation                          |
| `tests`       | Test-only infrastructure                                                  |
| `build`       | Build, CI, dependencies, development tooling, and NuGet packaging         |
| `repo`        | Repository structure, metadata, and contribution rules                    |

Use the product area's scope for its tests, documentation, and demo examples, even when a commit changes only
those files. For example, use `tag-helpers: test URL generation` rather than `tests: add URL tests`,
and `http: correct the request header example` rather than `docs: correct the request header example`.
Reserve `tests`, `docs`, and `demo` for changes to their own infrastructure or for related changes across multiple product areas.

Use `internals` for shared collection, serialization, debugging, or compatibility code.
If the code belongs to a specific product area, use that area's scope instead.
Use `hosting` for dependency registration, endpoint mapping, and serving the companion script.
Use `script` for browser behavior that does not belong to another product area.
Antiforgery behavior belongs to `config`, even when its implementation spans Tag Helpers and the companion script.

Split unrelated changes into separate commits. If one atomic change spans multiple areas,
choose the scope of the main product area affected. Do not use a type prefix or a ticket number as the scope.
Put ticket references and implementation details in the message body when needed.
