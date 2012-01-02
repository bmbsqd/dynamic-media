using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using System.Linq;

namespace Bombsquad.DynamicMedia.Markdown
{
    public class MarkdownMediaTransformerFactory : IMediaTransformerFactory
    {
        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!string.Equals(originalFormat.ContentType, "text/html", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = null;
                return false;
            }

            var path = request.Url.AbsolutePath;
            if (!path.Contains(".md.") && !path.Contains(".markdown.") )
            {
                mediaTransformer = null;
                return false;
            }
            
            Func<string,string> modifyAbsolutePathFunc = (absolutePath => absolutePath.Replace(".html", ""));

            mediaTransformer = new MarkdownMediaTransformer(modifyAbsolutePathFunc, originalFormat);
            return true;
        }
    }
}