# Markdown Reader

A simple Windows desktop app (WPF, .NET 8) to open and read Markdown (`.md`) files
with clean, comfortable, book-like formatting.

## What you need before running this

1. **.NET 8 SDK** (or newer) — download from:
   https://dotnet.microsoft.com/download/dotnet/8.0

2. **WebView2 Runtime** — this comes pre-installed on Windows 10/11 (it ships with
   Microsoft Edge). If for some reason it's missing, get it from:
   https://developer.microsoft.com/microsoft-edge/webview2/

That's it — no Visual Studio required, though you're welcome to open the project
in Visual Studio / VS Code / Rider if you prefer.

## How to run it

1. Open a terminal (PowerShell or cmd) in this folder (the one containing
   `MarkdownReader.csproj`).
2. Restore and run:

   ```
   dotnet restore
   dotnet run
   ```

   The first `dotnet restore` will download the two required libraries
   (Markdig for Markdown parsing, WebView2 for rendering) automatically.

3. The app window will open. Click **"📂 Open Markdown File"** and pick a `.md`
   file, or just drag and drop a `.md` file onto the window.

   A `sample.md` file is included in this folder if you want to try it
   immediately — just drag it into the app.

## Building a standalone .exe (optional)

If you want a single .exe you can double-click without needing `dotnet run`:

```
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The resulting `.exe` will be in:
`bin\Release\net8.0-windows\win-x64\publish\`

## Project structure

```
MarkdownReader/
├── MarkdownReader.csproj   ← project + dependencies
├── App.xaml / App.xaml.cs  ← app startup
├── MainWindow.xaml         ← UI layout (toolbar + reading area)
├── MainWindow.xaml.cs      ← logic: open file, convert markdown, render
└── sample.md               ← test file
```

## Customizing

- **Default font size**: change `_fontSize = 18` in `MainWindow.xaml.cs`.
- **Colors / theme**: edit the `<style>` block inside `BuildHtmlDocument(...)`
  in `MainWindow.xaml.cs` — it's plain CSS.
- **Light mode**: swap the dark hex colors (`#1e1e1e`, `#e0e0e0`, etc.) for
  light ones if you'd prefer a white background.
