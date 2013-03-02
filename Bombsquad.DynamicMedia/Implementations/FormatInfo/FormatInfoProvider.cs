using System;
using System.Collections.Generic;
using Bombsquad.DynamicMedia.Configuration;
using System.Linq;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.FormatInfo
{
	public class FormatInfoProvider : IFormatInfoProvider
    {
        private readonly IDictionary<string, FormatInfoElement> m_lookupFromExtension;

        public FormatInfoProvider(FormatInfoResolverConfiguration configuration)
        {
            m_lookupFromExtension = configuration.Mappings.ToDictionary(m => m.Extension, StringComparer.InvariantCultureIgnoreCase);
        }

        public IFormatInfo ResolveFromExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension) || !extension.StartsWith("."))
            {
                return null;
            }

            FormatInfoElement infoElement;
            if (!m_lookupFromExtension.TryGetValue(extension, out infoElement))
            {
                return null;
            }

            return new FormatInfo
            {
                Extension = extension.ToLower(),
                ContentType = infoElement.ContentType.ToLower(),
                AllowCompression = infoElement.AllowCompression,
                ClientCacheMaxAge = infoElement.ClientCacheMaxAge.HasValue
                    ? TimeSpan.FromSeconds(infoElement.ClientCacheMaxAge.Value)
                    : (TimeSpan?)null
            };
        }

        private class FormatInfo : IFormatInfo
        {
            public string Extension { get; set; }
            public string ContentType { get; set; }
            public bool AllowCompression { get; set; }
            public TimeSpan? ClientCacheMaxAge { get; set; }
        }
    }
}