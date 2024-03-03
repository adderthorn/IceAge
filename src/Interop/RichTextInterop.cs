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
using System.Text.RegularExpressions;

namespace IceAge.Interop;
internal class RichTextInterop
{
    private const char kHellip = (char)8230;

    private string _content;
    private readonly bool _shortenHyperlinks;
    private readonly HtmlDocument _htmlDocument;

    public RichTextBlock RichTextBlock { get; set; }
    public string Content
    {
        get => _content;
        set
        {
            if (_content == value)
                return;
            _content = value;
            RichTextBlock.Blocks.Clear();
            _htmlDocument.LoadHtml(ReplaceWhitespace(_content));
            var inlines = ParseNodes(_htmlDocument.DocumentNode.ChildNodes, RichTextBlock.Blocks);
            if (inlines.Count > 0)
            {
                var p = new Paragraph();
                foreach (var inline in inlines)
                    p.Inlines.Add(inline);
                RichTextBlock.Blocks.Add(p);
            }
        }
    }

    public RichTextInterop(RichTextBlock richTextBlock, bool shortenHyperlinks)
    {
        this.RichTextBlock = richTextBlock;
        this._shortenHyperlinks = shortenHyperlinks;
        _htmlDocument = new HtmlDocument();
    }

    private IList<Inline> ParseNodes(IEnumerable<HtmlNode> nodes, BlockCollection blocks)
    {
        IList<Inline> inlines = new List<Inline>();
        foreach (var node in nodes)
        {
            bool checkForChildNodes = false;
            if (node.NodeType == HtmlNodeType.Element)
            {
                switch (node.Name.ToLower())
                {
                    case "p":
                        if (inlines.Count > 0)
                        {
                            inlines.Add(new LineBreak());
                            var p = new Paragraph();
                            foreach (var inline in inlines)
                                p.Inlines.Add(inline);
                            blocks.Add(p);
                        }
                        inlines.Clear();
                        checkForChildNodes = true;
                        break;
                    case "a":
                        var a = ParseHyperlink(node, blocks);
                        if (a != null)
                            inlines.Add(a);
                        break;
                    case "div":
                        checkForChildNodes = true;
                        break;
                    case "br":
                        inlines.Add(new LineBreak());
                        break;
                    default:
                        var run = GetRun(node);
                        if (run != null)
                            inlines.Add(run);
                        break;
                }
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                var run = GetRun(node);
                if (run != null)
                    inlines.Add(run);
            }

            if (checkForChildNodes && node.HasChildNodes)
            {
                inlines = ParseNodes(node.ChildNodes, blocks);
            }
        }
        return inlines;
    }

    private Run GetRun(HtmlNode node)
    {
        if (string.IsNullOrEmpty(node.InnerText) || node.HasClass("invisible"))
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
        if (node.HasClass("ellipsis"))
        {
            run.Text += kHellip;
        }
        return run;
    }

    private Hyperlink ParseHyperlink(HtmlNode node, BlockCollection blocks)
    {
        var href = node.Attributes["href"];
        if (href == null)
            return null;

        var a = new Hyperlink();
        a.NavigateUri = new Uri(href.Value);
        if (_shortenHyperlinks && node.HasChildNodes)
        {
            var inlines = ParseNodes(node.ChildNodes, blocks);
            foreach (var inline in inlines)
                a.Inlines.Add(inline);
        }
        else
        {
            var run = GetRun(node);
            if (run != null)
                a.Inlines.Add(run);
        }
        return a;
    }

    private string ReplaceWhitespace(string text)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '\r' || c == '\n' || c == '\t')
                continue;
            sb.Append(c);
        }
        return Regex.Replace(sb.ToString(), @"\s+", " ");
    }
}
