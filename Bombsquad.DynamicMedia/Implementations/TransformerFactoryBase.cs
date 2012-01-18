using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
{
    public abstract class TransformerFactoryBase : IMediaTransformerFactory
    {
        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if (!CanHandleFormat(originalFormat))
            {
                mediaTransformer = null;
                return false;
            }

            if(!IsValidFilePath(request.Url.AbsolutePath))
            {
                mediaTransformer = null;
                return false;
            }

            mediaTransformer = new MediaTransformer(originalFormat, this);
            return true;
        }

        protected abstract bool IsValidFilePath(string absolutePath);
        protected abstract bool CanHandleFormat(IFormatInfo format);
        protected abstract MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream);
        protected virtual string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath;
        }
      
        private class MediaTransformer : IMediaTransformer
        {
            private readonly TransformerFactoryBase _transformerFactory;

            public MediaTransformer(IFormatInfo outputFormat, TransformerFactoryBase transformerFactory)
            {
                _transformerFactory = transformerFactory;
                OutputFormat = outputFormat;
            }

            public MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
            {
                return _transformerFactory.TransformStream(request, stream, out transformedStream);
            }

            public IFormatInfo OutputFormat { get; private set; }

            public string ModifyAbsolutePath(string absolutePath)
            {
                return _transformerFactory.ModifyAbsolutePath(absolutePath);
            }
        }
    }
}