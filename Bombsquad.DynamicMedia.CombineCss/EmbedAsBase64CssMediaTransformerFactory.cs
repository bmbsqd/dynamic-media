using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class EmbedAsBase64CssMediaTransformerFactory : IMediaTransformerFactory
    {
        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!request.Url.AbsolutePath.Contains(".embed."))
            {
                mediaTransformer = null;
                return false;
            }

            if (!string.Equals(originalFormat.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = null;
                return false;
            }

            Func<string, string> modifyAbsolutePathFunc = (absolutePath => absolutePath.Replace(".embed", ""));
            mediaTransformer = new EmbedAsBase64CssMediaTransformer(request.Url, originalFormat, modifyAbsolutePathFunc);
            return true;
        }
    }
}