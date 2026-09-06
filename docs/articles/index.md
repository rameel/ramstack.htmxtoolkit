<div class="docs-hero">
  <p class="docs-eyebrow">ASP.NET Core + HTMX</p>
  <h1>HtmxToolkit guides</h1>
  <p>Build server-rendered interactions with typed requests, fluent responses, Razor Tag Helpers, version-aware configuration, and automatic antiforgery.</p>
  <div class="docs-hero__actions">
    <a class="docs-button" href="getting-started.md">Get started</a>
    <a class="docs-button docs-button--secondary" href="../api/index.md">API reference</a>
  </div>
</div>

These guides focus on complete application tasks; the API reference documents individual types and members.

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
    <span>Target the correct markup for HTMX 1.x, 2.x, or 4.x.</span>
  </a>
</div>

## Build an integration

<div class="guide-grid guide-grid--two">
  <a class="guide-card" href="pages-and-fragments.md"><strong>Full pages and fragments</strong><span>Support normal navigation and HTMX from the same application.</span></a>
  <a class="guide-card" href="requests.md"><strong>Read HTMX requests</strong><span>Detect requests, inspect typed headers, and select MVC actions.</span></a>
  <a class="guide-card" href="responses.md"><strong>Control HTMX responses</strong><span>Choose targets, swaps, navigation, history, and client events.</span></a>
  <a class="guide-card" href="tag-helpers.md"><strong>Use Tag Helpers</strong><span>Generate URLs, values, headers, and request options from Razor.</span></a>
</div>

## Configure and operate

<div class="guide-grid guide-grid--two">
  <a class="guide-card" href="configuration.md"><strong>Application configuration</strong><span>Render strongly typed global settings for the selected version.</span></a>
  <a class="guide-card" href="version-compatibility.md"><strong>Version compatibility</strong><span>Compare requests, configuration, events, and swaps across releases.</span></a>
  <a class="guide-card" href="antiforgery.md"><strong>Antiforgery and assets</strong><span>Protect non-GET requests and deploy the companion script.</span></a>
  <a class="guide-card" href="recipes.md"><strong>Common recipes</strong><span>Implement validation, polling, redirects, and cross-component updates.</span></a>
  <a class="guide-card" href="troubleshooting.md"><strong>Troubleshooting</strong><span>Diagnose failures from generated markup and browser network data.</span></a>
</div>

> [!TIP]
> If an application already uses HTMX, begin with [Choose an HTMX version](choosing-version.md), then add only the toolkit features it needs.
