using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bombsquad.DynamicMedia.Contracts.ETag;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
    public class FileSystemMediaCache : FileSystemBasedMediaCache
    {
        private readonly DirectoryInfo m_cacheRoot;

        public FileSystemMediaCache(IFileInfoETagCalculator fileInfoETagCalculator, DirectoryInfo cacheRoot) 
            : base(fileInfoETagCalculator)
        {
            m_cacheRoot = cacheRoot;
        }

        private string GetCacheFileName(string cacheKey, IFormatInfo outputFormat)
        {
            var inputBytes = Encoding.UTF8.GetBytes(cacheKey);
            var hash = MD5.Create().ComputeHash(inputBytes);
            var relativeUrl = new Guid(hash).ToString().Replace('-', '\\') + outputFormat.Extension;
            return Path.Combine(m_cacheRoot.FullName, relativeUrl);
        }

        protected override FileInfo GetCacheFileInfo(string path, IFormatInfo outputFormat)
        {
            return new FileInfo(GetCacheFileName(path, outputFormat));
        }
    }
}