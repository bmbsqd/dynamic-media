using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Microsoft.Win32;

namespace Bombsquad.DynamicMedia.Implementations
{
    public class RegistryFormatInfoResolver : IFormatInfoResolver
    {
        private readonly IDictionary<string, string> _specialExtensionsToContentTypeLookup;

        public RegistryFormatInfoResolver()
        {
            _specialExtensionsToContentTypeLookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddSpecialFormatInfo(string extension, string contentType)
        {
            _specialExtensionsToContentTypeLookup.Add(extension, contentType);
        }

        public IFormatInfo ResolveFromRequest(HttpRequestBase request)
        {
            var extension = Path.GetExtension(request.Url.AbsolutePath);

            if (string.IsNullOrEmpty(extension) || !extension.StartsWith("."))
            {
                return null;
            }

            string contentType;
            if(_specialExtensionsToContentTypeLookup.TryGetValue(extension, out contentType))
            {
                return new FormatInfo
                {
                    Extension = extension.ToLower(),
                    ContentType = contentType.ToLower()
                };
            }

            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
            {
                contentType = key.GetValue("Content Type").ToString();
                return new FormatInfo
                {
                    Extension = extension.ToLower(),
                    ContentType = contentType.ToLower()
                };
            }
        }
    }
}