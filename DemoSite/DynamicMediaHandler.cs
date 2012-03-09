using System.Configuration;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia;
using Bombsquad.DynamicMedia.CombineCss;
using Bombsquad.DynamicMedia.Configuration;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Storage;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Imaging;
using Bombsquad.DynamicMedia.Implementations.Cache;
using Bombsquad.DynamicMedia.Implementations.FormatInfo;
using Bombsquad.DynamicMedia.Implementations.ResultHandlers;
using Bombsquad.DynamicMedia.Implementations.Storage;
using Bombsquad.DynamicMedia.Implementations.Transformation;
using Bombsquad.DynamicMedia.Less;
using Bombsquad.DynamicMedia.Markdown;
using Bombsquad.DynamicMedia.Minify;

namespace DemoSite
{
    public class DynamicMediaHandler : DynamicMediaHandlerBase
    {
        private readonly IMediaCache _mediaCache;
        private readonly IStorageBackend _storageBackend;
        private readonly IMediaTransformerFactory _mediaTransformerFactory;
        private readonly FormatInfoResolver _formatInfoResolver;

        public DynamicMediaHandler()
        {
            var storageRoot = new DirectoryInfo(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["StorageRoot"]));

            _mediaCache = new NullMediaCache();
            _storageBackend = new FileSystemStorageBackend(storageRoot);
            _mediaTransformerFactory = new CompositeMediaTransformerFactory(new IMediaTransformerFactory[]
            {
                new XmlExifImageInfoMediaTransformer(), 
                new JsonExifImageInfoMediaTransformer(), 
                new ImageMediaTransformerFactory(),
                new CssLessMediaTransformerFactory(),
                new CombineCssMediaTransformerFactory(),
                new EmbedAsBase64CssMediaTransformerFactory(), 
                new CssMinifyingMediaTransformerFactory(),
                new JavascriptMinifyingMediaTransformerFactory(), 
                new MarkdownMediaTransformerFactory()
            });

            _formatInfoResolver = new FormatInfoResolver((FormatInfoResolverConfiguration)ConfigurationManager.GetSection("dynamicMediaFormatMappings"));
        }

        protected override IMediaCache MediaCache { get { return _mediaCache; } }
        protected override IStorageBackend StorageBackend { get { return _storageBackend; } }
        protected override IMediaTransformerFactory MediaTransformerFactory { get { return _mediaTransformerFactory; } }
        protected override IFormatInfoResolver FormatInfoResolver { get { return _formatInfoResolver; } }

        protected override IResultHandler ResultHandler
        {
            get
            {
                return new CompositeResultHandler
                (
                    new NotModifiedResultHandler(),
                    new DefaultResultHandler()
                );
            }
        }
    }
}