using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
{
    public abstract class FileSystemBasedMediaCache : IMediaCache
    {
        public bool TryServeRequestFromCache(HttpRequestBase request, HttpResponseBase response, IFormatInfo outputFormat)
        {
            var cacheFile = GetCacheFileInfo(request, outputFormat);

            if (!cacheFile.Exists)
            {
                return false;
            }

            if(TrySendNotModifiedResponse(cacheFile, request, response))
            {
                return true;
            }

            response.Cache.SetLastModified(cacheFile.LastWriteTime);
            response.ContentType = outputFormat.ContentType;
            response.TransmitFile(cacheFile.FullName);
            return true;
        }

        private static bool TrySendNotModifiedResponse(FileInfo cacheFile, HttpRequestBase request, HttpResponseBase response)
        {
            var ifModifiedSince = request.Headers["if-modified-since"];
            DateTime modifiedSince;
            if (string.IsNullOrEmpty(ifModifiedSince) || !DateTime.TryParse(ifModifiedSince, out modifiedSince))
            {
                return false;
            }

            if (modifiedSince > cacheFile.LastWriteTime.Subtract(TimeSpan.FromSeconds(1)))
            {
                response.StatusCode = 304;
                response.SuppressContent = true;
                return true;
            }

            return false;
        }

        public void AddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat)
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
        }

        protected abstract FileInfo GetCacheFileInfo(HttpRequestBase request, IFormatInfo outputFormat);
    }
}