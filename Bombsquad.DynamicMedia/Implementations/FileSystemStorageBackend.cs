using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
{
    public class FileSystemStorageBackend : IStorageBackend
    {
        private readonly DirectoryInfo _storageRoot;

        public FileSystemStorageBackend(DirectoryInfo storageRoot)
        {
            _storageRoot = storageRoot;
        }

        public bool TryGetOriginalStream(HttpRequestBase request, IMediaTransformer mediaTransformer, out Stream outputStream)
        {
            FileInfo sourceImagePath;
            if (!TryFindPhysicalFile(request, mediaTransformer, out sourceImagePath))
            {
                outputStream = null;
                return false;
            }

            outputStream = new MemoryStream();

            using (var stream = sourceImagePath.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.CopyTo(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);
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
    }
}