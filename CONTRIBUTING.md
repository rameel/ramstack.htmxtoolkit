# Contributing

Bug reports and pull requests are welcome.

## Reporting bugs

Include a minimal reproduction, the expected and actual behavior, and the versions of HtmxToolkit, .NET, and HTMX.
For browser-side issues, also include the browser version and relevant console errors.

## Development setup

- Install the .NET 10 SDK to build the solution and run the tests and demo. The library itself targets .NET 6.
- Install Node.js and pnpm when working on the companion script.
- For documentation tooling and preview instructions, see [Building the documentation locally](docs/README.md).

Run the commands below from the repository root.

## Building and testing

Build the solution and run the tests:

```console
dotnet test
```

Compiler and analyzer warnings are treated as errors.

### Native AOT validation

The AOT validation project checks the built NuGet package and runs separately from the solution's unit tests.
On Linux x64, use [build-aot-validation.sh](build-aot-validation.sh) to build, pack, publish, and run the validation:

```console
bash build-aot-validation.sh
```

This requires the platform's [Native AOT prerequisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot/#prerequisites),
including a native compiler and development libraries.

## Code style

- Follow [.editorconfig](.editorconfig).
- Match the naming, formatting, and coding patterns of the surrounding code.
- Avoid unrelated reformatting when making functional changes.
- Name tests using the `Method_Condition_ExpectedResult` convention.

## Working on scripts and documentation

### Companion script

Edit `src/Ramstack.HtmxToolkit/wwwroot/htmx-toolkit.js`, then rebuild the script assets:

```console
pnpm install --frozen-lockfile
pnpm build
```

Include the updated `htmx-toolkit.js` and `htmx-toolkit.min.js` in the same commit.
Use the [demo application](README.md#demo) to check browser-side behavior with the affected HTMX versions.

### Documentation

Guides live in `docs/articles`. API examples are maintained in `docs/api-overwrites` and `docs/snippets`.
See [Building the documentation locally](docs/README.md) for build commands and instructions for adding API examples.

## Pull requests

- Keep each pull request focused on one coherent change.
- Explain what changed and why, and link to related issues when applicable.
- When fixing a reproducible bug, add a regression test. Add tests for new behavior where appropriate.
- Update the relevant documentation when public behavior changes.
- Preserve compatibility with the library's .NET 6 target and account for differences between supported HTMX versions.
  See [Version compatibility](docs/articles/version-compatibility.md).

Discuss substantial public API changes in an issue before starting implementation.

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
