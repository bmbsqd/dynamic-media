using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.ETag;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Implementations.Results;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
	public abstract class FileSystemBasedMediaCache : IMediaCache
    {
    	private readonly IFileInfoETagCalculator m_fileInfoETagCalculator;

    	protected FileSystemBasedMediaCache( IFileInfoETagCalculator fileInfoETagCalculator )
		{
			m_fileInfoETagCalculator = fileInfoETagCalculator;
		}

    	public bool TryServeRequestFromCache(string path, IFormatInfo outputFormat, out IResult result)
        {
            var cacheFile = GetCacheFileInfo(path, outputFormat);

            if (!cacheFile.Exists)
            {
                result = null;
                return false;
            }

    		var etag = m_fileInfoETagCalculator.CalculateETag( cacheFile );
            result = new TransmitFileResult(cacheFile.LastWriteTime, etag, cacheFile.Length, cacheFile.FullName);
            return true;
        }

        public bool TryAddToCache(string path, Func<Stream> stream, IFormatInfo outputFormat, out IAddToCacheResult result)
        {
            var cacheFile = GetCacheFileInfo(path, outputFormat);

            if (!cacheFile.Directory.Exists)
            {
                cacheFile.Directory.Create();
            }

            using (var fileStream = cacheFile.Create())
            {
                stream().CopyTo(fileStream);
            }

        	var etag = m_fileInfoETagCalculator.CalculateETag( cacheFile );
            result = new AddToCacheResult(cacheFile.LastWriteTime, etag);
            return true;
        }

		protected abstract FileInfo GetCacheFileInfo(string path, IFormatInfo outputFormat);

        public class AddToCacheResult : IAddToCacheResult
        {
            public AddToCacheResult(DateTime lastModified, string etag)
            {
            	LastModified = lastModified;
            	ETag = etag;
            }

        	public DateTime LastModified { get; private set; }
        	public string ETag { get; private set; }
        }
    }
}