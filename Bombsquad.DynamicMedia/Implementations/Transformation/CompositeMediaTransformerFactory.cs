using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Implementations.Transformation
{
    public class CompositeMediaTransformerFactory : IMediaTransformerFactory
    {
        private readonly IEnumerable<IMediaTransformerFactory> _transformerFactories;

        public CompositeMediaTransformerFactory(params IMediaTransformerFactory[] transformerFactories)
        {
            _transformerFactories = transformerFactories;
        }

        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, IFormatInfoProvider formatInfoProvider, out IMediaTransformer mediaTransformer)
        {
            var compositeTransformer = new CompositeMediaTransformer();
            var factories = _transformerFactories.ToList();
            AddTransformerToComposite(request, originalFormat, compositeTransformer, factories, formatInfoProvider);

            mediaTransformer = compositeTransformer;
            return compositeTransformer.Count > 0;
        }

        private static void AddTransformerToComposite(HttpRequestBase request, IFormatInfo originalFormat, CompositeMediaTransformer compositeTransformer, List<IMediaTransformerFactory> factories, IFormatInfoProvider formatInfoProvider)
        {
            foreach (var transformerFactory in factories)
            {
                IMediaTransformer transformer;
                if (transformerFactory.TryCreateTransformer(request, originalFormat, formatInfoProvider, out transformer))
                {
                    compositeTransformer.AddMediaTransformer(transformer);
                    factories.Remove(transformerFactory);
                    AddTransformerToComposite( request, transformer.OutputFormat, compositeTransformer, factories, formatInfoProvider);
                    return;
                }
            }
        }
    }
}