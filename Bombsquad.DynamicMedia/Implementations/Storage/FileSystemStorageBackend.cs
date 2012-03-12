using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.ETag;
using Bombsquad.DynamicMedia.Contracts.Storage;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations.Results;

namespace Bombsquad.DynamicMedia.Implementations.Storage
{
    public class FileSystemStorageBackend : IStorageBackend
    {
        private readonly DirectoryInfo m_storageRoot;
    	private readonly IFileInfoETagCalculator m_fileInfoETagCalculator;

    	public FileSystemStorageBackend(DirectoryInfo storageRoot, IFileInfoETagCalculator fileInfoETagCalculator)
        {
        	m_storageRoot = storageRoot;
        	m_fileInfoETagCalculator = fileInfoETagCalculator;
        }

    	public bool TryGetStorageFile(HttpRequestBase request, IMediaTransformer mediaTransformer, out IStorageFile storageFile)
        {
            FileInfo sourceImagePath;
            if (!TryFindPhysicalFile(request, mediaTransformer, out sourceImagePath))
            {
                storageFile = null;
                return false;
            }
    		
			var eTag = m_fileInfoETagCalculator.CalculateETag( sourceImagePath );
			storageFile = new StorageFile( sourceImagePath, eTag );
            return true;
        }

        private bool TryFindPhysicalFile(HttpRequestBase request, IMediaTransformer mediaTransformer, out FileInfo sourceImagePath)
        {
            var path = GetAbsolutePath(request);

            if (mediaTransformer != null)
            {
                path = mediaTransformer.ModifyAbsolutePath(path);
            }

            sourceImagePath = new FileInfo(Path.Combine(m_storageRoot.FullName, path));

            return sourceImagePath.Exists;
        }

        private static string GetAbsolutePath(HttpRequestBase request)
        {
            var absolutePath = request.Url.AbsolutePath;
            absolutePath = absolutePath.Replace('/', '\\');
            absolutePath = absolutePath.TrimStart('\\');
            return absolutePath;
        }

        private class StorageFile : TransmitFileResult, IStorageFile
        {
        	private readonly FileInfo _file;
        	private FileStream _stream;

        	public StorageFile(FileInfo file, string etag) 
				: base(file, etag)
            {
            	_file = file;
            }

        	public Stream GetStream()
        	{
				if( _stream == null)
				{
					_stream = _file.Open( FileMode.Open, FileAccess.Read, FileShare.Read );
					return _stream;
				}

        		_stream.Seek( 0, SeekOrigin.Begin );
        		return _stream;
        	}
        }
    }
}