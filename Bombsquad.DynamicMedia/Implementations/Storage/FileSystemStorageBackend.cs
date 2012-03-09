using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Storage;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations.Results;

namespace Bombsquad.DynamicMedia.Implementations.Storage
{
    public class FileSystemStorageBackend : IStorageBackend
    {
        private readonly DirectoryInfo _storageRoot;

        public FileSystemStorageBackend(DirectoryInfo storageRoot )
        {
            _storageRoot = storageRoot;
        }

        public bool TryGetOriginalStream(HttpRequestBase request, IMediaTransformer mediaTransformer, out IOriginal original)
        {
            FileInfo sourceImagePath;
            if (!TryFindPhysicalFile(request, mediaTransformer, out sourceImagePath))
            {
                original = null;
                return false;
            }

            var outputStream = new MemoryStream();

            using (var stream = sourceImagePath.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.CopyTo(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            original = new Original(outputStream, sourceImagePath.LastWriteTime);
            return true;
        }

        public bool TryServeOriginal(HttpRequestBase request, out IResult result)
        {
            FileInfo sourceImagePath;
            if (!TryFindPhysicalFile(request, null, out sourceImagePath))
            {
                result = null;
                return false;
            }

            result = new TransmitFileResult(sourceImagePath.LastWriteTime, sourceImagePath.FullName);
            return true;
        }

        private bool TryFindPhysicalFile(HttpRequestBase request, IMediaTransformer mediaTransformer, out FileInfo sourceImagePath)
        {
            var path = GetAbsolutePath(request);

            if (mediaTransformer != null)
            {
                path = mediaTransformer.ModifyAbsolutePath(path);
            }

            sourceImagePath = new FileInfo(Path.Combine(_storageRoot.FullName, path));

            return sourceImagePath.Exists;
        }

        private static string GetAbsolutePath(HttpRequestBase request)
        {
            var absolutePath = request.Url.AbsolutePath;
            absolutePath = absolutePath.Replace('/', '\\');
            absolutePath = absolutePath.TrimStart('\\');
            return absolutePath;
        }

        private class Original : IOriginal
        {
            public Original(Stream stream, DateTime lastModified)
            {
                Stream = stream;
                LastModified = lastModified;
            }

            public Stream Stream { get; private set; }
            public DateTime LastModified { get; private set; }
        }
    }
}