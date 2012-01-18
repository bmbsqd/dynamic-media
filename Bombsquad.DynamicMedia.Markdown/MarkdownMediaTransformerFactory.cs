using System;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Implementations;

namespace Bombsquad.DynamicMedia.Markdown
{
    public class MarkdownMediaTransformerFactory : TransformerFactoryTextBase
    {
        private readonly MarkdownSharp.Markdown _markdown;

        public MarkdownMediaTransformerFactory()
        {
            _markdown = new MarkdownSharp.Markdown();
        }

        protected override MediaTransformResult TransformText(string text, out string transformedText)
        {
            transformedText = _markdown.Transform(text);
            return MediaTransformResult.Success;
        }

        protected override string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath.Replace(".html", "");
        }

        protected override bool IsValidFilePath(string absolutePath)
        {
            return absolutePath.Contains(".md.") || absolutePath.Contains(".markdown.");
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.ContentType, "text/html", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}