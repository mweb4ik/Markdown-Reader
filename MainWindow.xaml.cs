using System;
using System.IO;
using System.Windows;
using Markdig;
using Microsoft.Win32;

namespace MarkdownReader
{
    public partial class MainWindow : Window
    {
        private string? _currentFilePath;
        private int _fontSize = 18; // comfortable reading default
        private readonly MarkdownPipeline _pipeline;

        public MainWindow()
        {
            InitializeComponent();

            // Build the Markdig pipeline with the extensions that matter for comfortable reading:
            // tables, footnotes, task lists, auto-links, etc.
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .Build();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the WebView2 environment ahead of time so the first file
            // opens instantly with no delay.
            await MarkdownView.EnsureCoreWebView2Async(null);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open Markdown File",
                Filter = "Markdown files (*.md;*.markdown;*.txt)|*.md;*.markdown;*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadFile(dialog.FileName);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFilePath != null)
            {
                LoadFile(_currentFilePath);
            }
        }

        private void FontUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fontSize < 32)
            {
                _fontSize += 2;
                RerenderCurrentFile();
            }
        }

        private void FontDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fontSize > 12)
            {
                _fontSize -= 2;
                RerenderCurrentFile();
            }
        }

        private void RerenderCurrentFile()
        {
            if (_currentFilePath != null)
            {
                LoadFile(_currentFilePath, resetScroll: false);
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    LoadFile(files[0]);
                }
            }
        }

        private void LoadFile(string path, bool resetScroll = true)
        {
            try
            {
                string markdownText = File.ReadAllText(path);
                string html = Markdown.ToHtml(markdownText, _pipeline);
                string fullHtml = BuildHtmlDocument(html, _fontSize);

                _currentFilePath = path;
                FileNameText.Text = Path.GetFileName(path);
                FileNameText.Foreground = System.Windows.Media.Brushes.White;
                ReloadButton.IsEnabled = true;
                PlaceholderPanel.Visibility = Visibility.Collapsed;

                MarkdownView.NavigateToString(fullHtml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not open this file:\n{ex.Message}",
                    "Error opening file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Wraps the converted Markdown HTML in a full HTML document with CSS
        /// designed for comfortable, book-like reading.
        /// </summary>
        private static string BuildHtmlDocument(string bodyHtml, int fontSize)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<style>
    html, body {{
        margin: 0;
        padding: 0;
        background-color: #1e1e1e;
    }}

    body {{
        font-family: 'Segoe UI', -apple-system, 'Georgia', serif;
        font-size: {fontSize}px;
        line-height: 1.7;
        color: #e0e0e0;
        max-width: 760px;
        margin: 0 auto;
        padding: 48px 32px 96px 32px;
    }}

    h1, h2, h3, h4, h5, h6 {{
        font-family: 'Segoe UI', -apple-system, sans-serif;
        color: #ffffff;
        font-weight: 600;
        margin-top: 1.6em;
        margin-bottom: 0.6em;
        line-height: 1.3;
    }}

    h1 {{ font-size: 1.9em; border-bottom: 1px solid #3a3a3a; padding-bottom: 0.3em; }}
    h2 {{ font-size: 1.5em; border-bottom: 1px solid #2e2e2e; padding-bottom: 0.25em; }}
    h3 {{ font-size: 1.25em; }}
    h4 {{ font-size: 1.1em; }}

    p {{ margin: 0.9em 0; }}

    a {{ color: #6cb6ff; text-decoration: none; }}
    a:hover {{ text-decoration: underline; }}

    strong {{ color: #ffffff; font-weight: 700; }}
    em {{ color: #e8e8e8; }}

    code {{
        background-color: #2d2d2d;
        color: #f0c674;
        padding: 0.15em 0.4em;
        border-radius: 4px;
        font-family: 'Cascadia Code', Consolas, monospace;
        font-size: 0.88em;
    }}

    pre {{
        background-color: #2a2a2a;
        border: 1px solid #3a3a3a;
        border-radius: 8px;
        padding: 16px;
        overflow-x: auto;
        line-height: 1.5;
    }}

    pre code {{
        background: none;
        padding: 0;
        color: #d4d4d4;
    }}

    blockquote {{
        border-left: 4px solid #555;
        margin: 1em 0;
        padding: 0.4em 1em;
        color: #b0b0b0;
        background-color: #252525;
        border-radius: 0 6px 6px 0;
    }}

    ul, ol {{
        padding-left: 1.6em;
    }}

    li {{ margin: 0.4em 0; }}

    table {{
        border-collapse: collapse;
        width: 100%;
        margin: 1.2em 0;
    }}

    th, td {{
        border: 1px solid #3a3a3a;
        padding: 8px 12px;
        text-align: left;
    }}

    th {{
        background-color: #2a2a2a;
        color: #ffffff;
    }}

    tr:nth-child(even) {{
        background-color: #232323;
    }}

    img {{
        max-width: 100%;
        border-radius: 6px;
    }}

    hr {{
        border: none;
        border-top: 1px solid #3a3a3a;
        margin: 2em 0;
    }}

    ::selection {{
        background-color: #264f78;
    }}
</style>
</head>
<body>
{bodyHtml}
</body>
</html>";
        }
    }
}
