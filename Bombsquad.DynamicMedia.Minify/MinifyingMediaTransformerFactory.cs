using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Microsoft.Ajax.Utilities;

namespace Bombsquad.DynamicMedia.Minify
{
    public class MinifyingMediaTransformerFactory : IMediaTransformerFactory
    {
        private readonly Minifier _minifier;

        public MinifyingMediaTransformerFactory()
        {
            _minifier = new Minifier();
        }

        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!request.Url.AbsolutePath.Contains(".min."))
            {
                mediaTransformer = null;
                return false;
            }

            Func<string, string> modifyAbsolutePathFunc = (absolutePath => absolutePath.Replace(".min", ""));

            if (string.Equals(originalFormat.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = new MinifyingMediaTransformer(originalFormat, _minifier.MinifyStyleSheet, modifyAbsolutePathFunc);
                return true;
            }

            if (string.Equals(originalFormat.ContentType, "text/javascript", StringComparison.InvariantCultureIgnoreCase))
            {
                mediaTransformer = new MinifyingMediaTransformer(originalFormat, _minifier.MinifyJavaScript, modifyAbsolutePathFunc);
                return true;
            }

            mediaTransformer = null;
            return false;
        }
    }
}
