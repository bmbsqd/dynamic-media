using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Implementations.Transformation
{
    internal class CompositeMediaTransformer : IMediaTransformer
    {
        private readonly List<IMediaTransformer> _transformers;

        public CompositeMediaTransformer()
        {
            _transformers = new List<IMediaTransformer>();
        }

        public int Count
        {
            get { return _transformers.Count; }
        }

        public void AddMediaTransformer(IMediaTransformer transformer)
        {
            _transformers.Add(transformer);
        }

        public MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
        {
            transformedStream = stream;
            var worstResult = MediaTransformResult.Success;
            
            foreach (var transformer in _transformers)
            {
                var mediaTransformResult = transformer.TransformStream(request, transformedStream, out transformedStream);
                if(mediaTransformResult > worstResult)
                {
                    worstResult = mediaTransformResult;
                }
            }

            return worstResult;
        }

        public IFormatInfo OutputFormat
        {
            get
            {
                var lastTransformer = _transformers.LastOrDefault();
                return lastTransformer == null ? null : lastTransformer.OutputFormat;
            }
        }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _transformers.Aggregate(absolutePath, (current, transformer) => transformer.ModifyAbsolutePath(current));
        }
    }
}