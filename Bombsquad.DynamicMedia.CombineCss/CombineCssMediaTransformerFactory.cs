using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class CombineCssMediaTransformerFactory : IMediaTransformerFactory
    {
        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!request.Url.AbsolutePath.Contains(".combine."))
            {
                mediaTransformer = null;
                return false;
            }

            if (!string.Equals(originalFormat.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = null;
                return false;
            }

            Func<string, string> modifyAbsolutePathFunc = (absolutePath => absolutePath.Replace(".combine", ""));
            mediaTransformer = new CombineCssMediaTransformer(request.Url, originalFormat, modifyAbsolutePathFunc);
            return true;
        }
    }
}
