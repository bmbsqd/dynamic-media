using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
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

        public Stream TransformStream(HttpRequestBase request, Stream stream)
        {
            return _transformers.Aggregate(stream, (current, transformer) => transformer.TransformStream(request, current));
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