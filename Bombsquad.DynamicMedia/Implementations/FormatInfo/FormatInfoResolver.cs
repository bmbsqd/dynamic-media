using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Configuration;
using System.Linq;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.FormatInfo
{
    public class FormatInfoResolver : IFormatInfoResolver
    {
        private readonly IDictionary<string, string> _specialExtensionsToContentTypeLookup;

        public FormatInfoResolver(FormatInfoResolverConfiguration configuration)
        {
            _specialExtensionsToContentTypeLookup = configuration.Mappings.ToDictionary(m => m.Extension, m => m.ContentType, StringComparer.InvariantCultureIgnoreCase);
        }

        public IFormatInfo ResolveFromRequest(HttpRequestBase request)
        {
            var extension = Path.GetExtension(request.Url.AbsolutePath);

            if (string.IsNullOrEmpty(extension) || !extension.StartsWith("."))
            {
                return null;
            }

            string contentType;
            if (!_specialExtensionsToContentTypeLookup.TryGetValue(extension, out contentType))
            {
                return null;
            }

            return new FormatInfo
            {
                Extension = extension.ToLower(),
                ContentType = contentType.ToLower()
            };
        }
    }
}