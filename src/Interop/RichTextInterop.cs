using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using HtmlAgilityPack;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Text;
using Windows.UI.Text;
using System.Diagnostics;

namespace IceAge.Interop;
internal class RichTextInterop
{
    private string _content;
    private readonly HtmlDocument _htmlDocument;

    public RichTextBlock RichTextBlock { get; set; }
    public string Content
    {
        get => _content;
        set
        {
            if (_content == value) return;
            _content = value;
            RichTextBlock.Blocks.Clear();
            _htmlDocument.LoadHtml(_content);
            foreach (var node in _htmlDocument.DocumentNode.ChildNodes)
            {
                parseChild(RichTextBlock.Blocks, node);
            }
        }
    }

    public RichTextInterop(RichTextBlock richTextBlock)
    {
        this.RichTextBlock = richTextBlock;
        _htmlDocument = new HtmlDocument();
    }

    private void parseChild(BlockCollection blocks, HtmlNode node)
    {
        if (node.Name.ToLower() == "p")
        {
            var paragraph = new Paragraph();
            foreach (var child in node.ChildNodes)
            {
                switch (child.Name.ToLower())
                {
                    case "a":
                        var link = new Hyperlink();
                        link.Inlines.Add(GetRun(child));
                        link.NavigateUri = new Uri(child.Attributes["href"].Value);
                        paragraph.Inlines.Add(link);
                        break;
                    case "br":
                        paragraph.Inlines.Add(new LineBreak());
                        break;
                    default:
                        var run = GetRun(child);
                        if (run != null)
                        {
                            paragraph.Inlines.Add(run);
                        }
                        break;
                }
                if (child.HasChildNodes)
                {
                    parseChild(blocks, child);
                }
            }
            paragraph.Inlines.Add(new LineBreak());
            blocks.Add(paragraph);
        }
    }

    private Run GetRun(HtmlNode node)
    {
        if (string.IsNullOrEmpty(node.InnerText))
            return null;
        var run = new Run { Text = HtmlEntity.DeEntitize(node.InnerText) };
        switch (node.Name.ToLower())
        {
            case "em":
            case "i":
                run.FontStyle = FontStyle.Italic;
                break;
            case "strong":
            case "b":
                run.FontWeight = FontWeights.Bold;
                break;
            case "u":
                run.TextDecorations = TextDecorations.Underline;
                break;
        }
        return run;
    }
}
