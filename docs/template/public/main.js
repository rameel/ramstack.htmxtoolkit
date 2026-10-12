// DocFX highlights code blocks with highlight.js, which has no Razor grammar.
// Register a small one so ```razor and [!code-razor[]] blocks are colored like HTML with C# in the Razor parts.

const NESTED_PARENS = String.raw`\((?:[^()\n]|\([^()\n]*\))*\)`;

function razor(hljs) {
  // @Model.Id, @user.Id, @Html.Raw("..."), @await Html.PartialAsync("..."), @RenderBody(), @(expression).
  // `\B` keeps e-mail addresses and package references such as htmx.org@2.0.11 from matching.
  const EXPRESSION = {
    className: "razor-expr",
    begin: new RegExp(
      String.raw`\B@(?:${NESTED_PARENS}|(?:await\s+)?(?!\d)[A-Za-z_]\w*(?:\??\.\w+|${NESTED_PARENS}|\[[^\]\n]*\])*)`)
  };

  return {
    name: "Razor",
    aliases: ["cshtml"],
    subLanguage: "xml",
    contains: [
      hljs.COMMENT(/@\*/, /\*@/),
      { begin: /@@/ },
      {
        // @model Foo, @using Foo, @addTagHelper *, Foo, @page, @namespace Foo, ...
        className: "keyword",
        begin: /\B@(?:addTagHelper|removeTagHelper|tagHelperPrefix|model|using(?!\s*\()|inject|inherits|implements|namespace|layout|page|attribute|preservewhitespace|rendermode|typeparam|section|functions|code)\b/
      },
      {
        // @if (...), @foreach (...), @using (...): keyword plus the C# in the rest of the line.
        className: "keyword",
        begin: /\B@(?:if|else|for|foreach|while|do|switch|try|catch|finally|lock|using)\b/,
        starts: { end: /$/, subLanguage: "csharp" }
      },
      {
        // @{ ... } with the closing brace at the start of a line.
        className: "keyword",
        begin: /\B@\{/,
        starts: { end: /^\}/, excludeEnd: true, subLanguage: "csharp" }
      },
      EXPRESSION,
      {
        // Attribute values, so expressions inside them keep the string color around them.
        className: "string",
        begin: /(?<==)"/,
        end: /"/,
        contains: [EXPRESSION]
      },
      {
        className: "string",
        begin: /(?<==)'/,
        end: /'/,
        contains: [EXPRESSION]
      }
    ]
  };
}

export default {
  configureHljs(hljs) {
    hljs.registerLanguage("razor", razor);
  }
};
