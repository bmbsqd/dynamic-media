using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
    public class FileSystemMediaCache : FileSystemBasedMediaCache
    {
        private readonly DirectoryInfo _cacheRoot;

        public FileSystemMediaCache(DirectoryInfo cacheRoot, bool cacheOriginals) 
            : base(cacheOriginals)
        {
            _cacheRoot = cacheRoot;
        }

        private string GetCacheFileName(string cacheKey, IFormatInfo outputFormat)
        {
            var inputBytes = Encoding.UTF8.GetBytes(cacheKey);
            var hash = MD5.Create().ComputeHash(inputBytes);
            var relativeUrl = new Guid(hash).ToString().Replace('-', '\\') + outputFormat.Extension;
            return Path.Combine(_cacheRoot.FullName, relativeUrl);
        }

        private string GetCacheKey(HttpRequestBase request)
        {
            return request.Url.PathAndQuery.ToLower();
        }

        protected override FileInfo GetCacheFileInfo(HttpRequestBase request, IFormatInfo outputFormat)
        {
            var cacheKey = GetCacheKey(request);
            return new FileInfo(GetCacheFileName(cacheKey, outputFormat));
        }
    }
}