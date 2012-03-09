using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Implementations.Transformation
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

            IFormatInfo outputFormat;
            if( !TryGetOutputFormat(request, originalFormat, out outputFormat) )
            {
                mediaTransformer = null;
                return false;
            }

            mediaTransformer = new MediaTransformer(outputFormat, this);
            return true;
        }

        protected abstract bool IsValidFilePath(string absolutePath);
        protected abstract bool CanHandleFormat(IFormatInfo format);
        protected abstract MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream);
        protected virtual string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath;
        }
        protected virtual bool TryGetOutputFormat(HttpRequestBase request, IFormatInfo originalFormat, out IFormatInfo outputFormat)
        {
            outputFormat = originalFormat;
            return true;
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