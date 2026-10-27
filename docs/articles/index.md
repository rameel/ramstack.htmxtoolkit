<div class="docs-hero">
  <p class="docs-eyebrow">ASP.NET Core + HTMX</p>
  <h1>HtmxToolkit guides</h1>
  <p>Load HTML fragments, submit forms, and update your pages with HTMX and ASP.NET Core.</p>
  <div class="docs-hero__actions">
    <a class="docs-button" href="getting-started.md">Get started</a>
    <a class="docs-button docs-button--secondary" href="../api/index.md">API reference</a>
  </div>
</div>

Choose a path below. For individual types and members, use the [API reference](../api/index.md).

New to HTMX? Read the short [Overview](overview.md) first. Then build one Razor Pages application through
three tutorials, with each step building on the previous one:

1. [Getting started](getting-started.md): load product details into the page.
2. [Forms, validation, and events](forms-and-validation.md): save a profile and refresh its summary.
3. [Loading states and request errors](loading-and-errors.md): show progress and recover from failed requests.

For another application style, use the complete [MVC](mvc.md) or [Minimal API](minimal-apis.md) example.
For an existing integration, browse the task guides below or [run the demo](samples.md).

## Start here

<div class="guide-grid">
  <a class="guide-card" href="overview.md">
    <span class="guide-card__label">Concept</span>
    <strong>Overview</strong>
    <span>Understand what the toolkit provides and where HTMX itself fits.</span>
  </a>
  <a class="guide-card" href="getting-started.md">
    <span class="guide-card__label">Tutorial</span>
    <strong>Getting started</strong>
    <span>Build a working interaction from installation through a server-rendered fragment.</span>
  </a>
  <a class="guide-card" href="choosing-version.md">
    <span class="guide-card__label">Decision</span>
    <strong>Choose an HTMX version</strong>
    <span>Target the correct markup for HTMX 1.9.x, 2.x, or 4.x.</span>
  </a>
</div>

## Build an integration

<div class="guide-grid guide-grid--two">
  <a class="guide-card" href="forms-and-validation.md"><strong>Forms, validation, and events</strong><span>Build a complete form that saves, shows errors, and refreshes another component.</span></a>
  <a class="guide-card" href="loading-and-errors.md"><strong>Loading states and request errors</strong><span>Show a loading message, disable repeated clicks, and recover from failed requests.</span></a>
  <a class="guide-card" href="pages-and-fragments.md"><strong>Full pages and fragments</strong><span>Support normal navigation and HTMX from the same application.</span></a>
  <a class="guide-card" href="requests.md"><strong>Read HTMX requests</strong><span>Detect requests, inspect typed headers, and select MVC actions.</span></a>
  <a class="guide-card" href="responses.md"><strong>Control HTMX responses</strong><span>Choose targets, swaps, navigation, history, and client events.</span></a>
  <a class="guide-card" href="tag-helpers.md"><strong>Use Tag Helpers</strong><span>Generate URLs, values, headers, and request options from Razor.</span></a>
</div>

## Configure and operate

<div class="guide-grid guide-grid--two">
  <a class="guide-card" href="configuration.md"><strong>Application configuration</strong><span>Render strongly typed global settings for the selected version.</span></a>
  <a class="guide-card" href="version-compatibility.md"><strong>Version compatibility</strong><span>Compare requests, configuration, events, and swaps across releases.</span></a>
  <a class="guide-card" href="antiforgery.md"><strong>Antiforgery and Toolkit script</strong><span>Send tokens, enable server validation, and see what the Toolkit script does.</span></a>
  <a class="guide-card" href="static-assets.md"><strong>Static assets and deployment</strong><span>Load the Toolkit script, configure caching, and publish the application.</span></a>
  <a class="guide-card" href="recipes.md"><strong>Common recipes</strong><span>Implement validation, polling, redirects, and cross-component updates.</span></a>
  <a class="guide-card" href="troubleshooting.md"><strong>Troubleshooting</strong><span>Diagnose failures from generated markup and browser network data.</span></a>
  <a class="guide-card" href="migrate-to-v2.md"><strong>Migrate from 1.x to 2.0</strong><span>Update registration, configuration, Tag Helpers, and response code for 2.0.</span></a>
</div>

> [!TIP]
> If an application already uses HTMX, begin with [Choose an HTMX version](choosing-version.md), then add only the toolkit features it needs.
