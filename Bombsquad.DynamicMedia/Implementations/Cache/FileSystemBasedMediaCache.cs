using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Implementations.Results;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
    public abstract class FileSystemBasedMediaCache : IMediaCache
    {
        protected FileSystemBasedMediaCache(bool cacheOriginals)
        {
            CacheOriginals = cacheOriginals;
        }

        public bool TryServeRequestFromCache(HttpRequestBase request, IFormatInfo outputFormat, out IResult result)
        {
            var cacheFile = GetCacheFileInfo(request, outputFormat);

            if (!cacheFile.Exists)
            {
                result = null;
                return false;
            }

            result = new TransmitFileResult(cacheFile.LastWriteTime, cacheFile.FullName);
            return true;
        }

        public bool TryAddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat, out IAddToCacheResult result)
        {
            var cacheFile = GetCacheFileInfo(request, outputFormat);

            if (!cacheFile.Directory.Exists)
            {
                cacheFile.Directory.Create();
            }

            using (var fileStream = cacheFile.Create())
            {
                stream.CopyTo(fileStream);
            }

            result = new AddToCacheResult(DateTime.Now);
            return true;
        }

        public bool CacheOriginals { get; private set; }

        protected abstract FileInfo GetCacheFileInfo(HttpRequestBase request, IFormatInfo outputFormat);

        public class AddToCacheResult : IAddToCacheResult
        {
            public AddToCacheResult(DateTime lastModified)
            {
                LastModified = lastModified;
            }

            public DateTime LastModified { get; private set; }
        }
    }
}