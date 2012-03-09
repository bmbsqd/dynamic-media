using System;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.Transformation;

namespace Bombsquad.DynamicMedia.Less
{
    public class CssLessMediaTransformerFactory : TransformerFactoryTextBase
    {
        protected override bool IsValidFilePath(string absolutePath)
        {
            return absolutePath.Contains(".less");
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override MediaTransformResult TransformText(string text, out string transformedText)
        {
            transformedText = dotless.Core.Less.Parse(text);
            return MediaTransformResult.Success;
        }

        protected override string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath.Replace(".css", "");
        }
    }
}