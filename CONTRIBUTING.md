# Contributing

Bug reports, fixes, and documentation improvements are welcome, including small ones.

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

The scripts in [`eng/`](eng/) run the same checks as CI, which runs all of them on every pull request.
Choose `Test`, `Aot`, `JavaScript`, or `Docs`, or use `All` to run them together.

- Linux and macOS: `bash eng/validate.sh <target>`
- Windows: `.\eng\validate.cmd <target>`

The `Aot` check validates the built NuGet package with Native AOT. It requires the platform's
[Native AOT prerequisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot/#prerequisites).

## Pull requests

- Keep each pull request focused on one goal. Its commits can use different scopes when they serve that goal.
  Do not mix unrelated changes into it.
- Several cleanups that do not change behavior, such as formatting, comment fixes, and readability improvements,
  can share one pull request when each is a separate commit. Send behavior changes found along the way
  as separate pull requests.
- Explain what changed and why, and link to related issues if there are any.
- When fixing a reproducible bug, add a regression test. Add tests for new behavior where appropriate.
- Update the documentation if your change affects how people use the library.
- Keep changes compatible with .NET 6 and the supported HTMX versions.
  See [Version compatibility](docs/articles/version-compatibility.md).

For major public API changes, open an issue first so we can agree on the approach before you spend time on it.

Use the [scoped commit style](#commit-messages) for pull request titles. Choose the scope with the
[same rules](#choosing-a-scope) as for a commit, treating the whole pull request as one change.
The title does not need to summarize every supporting commit.

## Commit messages

Use scoped commit messages:

```text
<scope>: <concise description>
```

The scope identifies the part of the project that changed. Write the description in the imperative mood,
starting with a lowercase letter and without a trailing period. Describe what the change does rather than
just naming its type.

```text
http: detect boosted navigation
tag-helpers: render HTMX 4 configuration modifiers
config: remove the HTMX 4 MetaCharacter option
```

Do not prefix messages with conventional commit types such as `feat`, `fix`, `refactor`, or `chore`,
and do not use ticket numbers as scopes. The description should make the type of change clear when it matters.
Put ticket references and implementation details in the message body when needed.

### Scopes

| Scope         | Area                                                                                      |
|---------------|-------------------------------------------------------------------------------------------|
| `http`        | HTTP request detection, headers, MVC selection, and response APIs                         |
| `tag-helpers` | Razor Tag Helpers                                                                         |
| `config`      | Application, version-specific, and antiforgery configuration                              |
| `hosting`     | Endpoint mapping, dependency registration, and server-side asset delivery                 |
| `script`      | Browser-side companion script behavior that does not belong to another product area       |
| `internals`   | Shared collection, serialization, debugging, and compatibility code                       |
| `docs`        | Documentation covering multiple product areas and DocFX tooling                           |
| `demo`        | Demo-only application structure and presentation                                          |
| `tests`       | Test-only infrastructure                                                                  |
| `build`       | Build, CI, analyzers, dependencies, development tooling, and NuGet packaging              |
| `repo`        | Repository structure and metadata, contribution and code style rules, cross-area cleanups |

Use the product area's scope for its tests, documentation, and demo examples, even when a commit changes only
those files. For example, use `tag-helpers: test URL generation` rather than `tests: add URL tests`,
and `http: correct the request header example` rather than `docs: correct the request header example`.
Reserve `tests`, `docs`, and `demo` for changes to their own infrastructure or for related changes across multiple product areas.

### Choosing a scope

Most commits change one area; use its scope from the table. When a commit touches several scopes,
use the first rule that applies:

1. **It changes behavior or public API.** Use the scope of the area where the change is visible,
   even if the implementation spans other areas. Antiforgery behavior belongs to `config`, even when
   its implementation spans Tag Helpers and the companion script. If several areas change, use the one
   whose documentation describes the change.
2. **It introduces or updates one thing that the rest of the commit adopts.** Use the scope of that thing:
   `internals: validate arguments with the Argument helper`,
   `build: enable analyzers and treat warnings as errors`, `repo: define formatting rules`.
3. **It only changes comments or documentation.** Use `docs`.
4. **Anything else**, such as formatting, renames, or file moves. Use `repo`.

Make each commit one logical change. Split logically separate changes into separate commits, even when they
share a scope: if the description needs "and" to join unrelated changes, the commit should be split.
Do not split a single change only to give each part its own scope, and do not list several scopes (`http, config:`).
