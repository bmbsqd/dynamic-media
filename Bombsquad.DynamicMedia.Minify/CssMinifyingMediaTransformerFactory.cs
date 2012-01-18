using System;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Implementations;
using Microsoft.Ajax.Utilities;

namespace Bombsquad.DynamicMedia.Minify
{
    public class CssMinifyingMediaTransformerFactory : TransformerFactoryTextBase
    {
        private readonly Minifier _minifier;

        public CssMinifyingMediaTransformerFactory()
        {
            _minifier = new Minifier();
        }

        protected override bool IsValidFilePath(string absolutePath)
        {
            return absolutePath.Contains(".min.");
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override MediaTransformResult TransformText(string text, out string transformedText)
        {
            transformedText = _minifier.MinifyStyleSheet(text);
            return MediaTransformResult.Success;
        }

        protected override string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath.Replace(".min", "");
        }
    }
}