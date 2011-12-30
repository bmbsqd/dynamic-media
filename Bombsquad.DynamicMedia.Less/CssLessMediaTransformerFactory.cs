using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Less
{
    public class CssLessMediaTransformerFactory : IMediaTransformerFactory
    {
        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!request.Url.AbsolutePath.Contains(".less."))
            {
                mediaTransformer = null;
                return false;
            }
            
            Func<string,string> modifyAbsolutePathFunc = (absolutePath => absolutePath.Replace(".css", ""));
            
            if (string.Equals(originalFormat.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = new CssLessMediaTransformer(modifyAbsolutePathFunc, originalFormat);
                return true;
            }

            mediaTransformer = null;
            return false;
        }
    }
}