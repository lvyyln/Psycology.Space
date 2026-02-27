# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the app (from solution root or server project)
dotnet run --project Psycology.Space

# Build the solution
dotnet build Psycology.Space.sln

# Publish
dotnet publish Psycology.Space/Psycology.Space.csproj -c Release

# Docker build
docker build -f Psycology.Space/Dockerfile -t psycology-space .
```

Dev URLs: `http://localhost:5032` / `https://localhost:7027`

## Architecture

This is a **Blazor Web App** (.NET 10) with Auto render mode, consisting of two projects:

- **`Psycology.Space/`** — ASP.NET Core server host. Contains server-side components, layout, and app shell. Registers both InteractiveServer and InteractiveWebAssembly render modes.
- **`Psycology.Space.Client/`** — Blazor WebAssembly client project. Contains components that run in the browser (e.g., `Counter.razor` with `@rendermode InteractiveAuto`).

### Render Mode Convention
- Pages in `Psycology.Space/Components/Pages/` render server-side by default (static SSR or `@attribute [StreamRendering]`)
- Pages in `Psycology.Space.Client/Pages/` use `@rendermode InteractiveAuto` (tries WebAssembly, falls back to SignalR)
- The router in `Routes.razor` includes both assemblies so all pages are discoverable

### Global Namespaces
Both projects have `_Imports.razor` files with pre-imported namespaces. No need to add `@using` for `Microsoft.AspNetCore.Components.*`, `Microsoft.JSInterop`, or the project's own namespaces in individual components.