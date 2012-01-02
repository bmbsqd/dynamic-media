using System.Configuration;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia;
using Bombsquad.DynamicMedia.CombineCss;
using Bombsquad.DynamicMedia.Configuration;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Imaging;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Less;
using Bombsquad.DynamicMedia.Minify;

namespace DemoSite
{
    public class RequestHandlerImpl : RequestHandler
    {
        private readonly IMediaCache _mediaCache;
        private readonly IStorageBackend _storageBackend;
        private readonly IMediaTransformerFactory _mediaTransformerFactory;
        private readonly FormatInfoResolver _formatInfoResolver;

        public RequestHandlerImpl()
        {
            var storageRoot = new DirectoryInfo(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["StorageRoot"]));
            var cacheRoot = new DirectoryInfo(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CacheRoot"]));

            //_mediaCache = new FileSystemMediaCache(cacheRoot);
            _mediaCache = new NullMediaCache();
            _storageBackend = new FileSystemStorageBackend(storageRoot);
            _mediaTransformerFactory = new CompositeMediaTransformerFactory(new IMediaTransformerFactory[]
            {
                new ImageInfoMediaTransformerFactory(),
                new ImageMediaTransformerFactory(),
                new CssLessMediaTransformerFactory(),
                new CombineCssMediaTransformerFactory(),
                new EmbedAsBase64CssMediaTransformerFactory(), 
                new MinifyingMediaTransformerFactory()
            });

            _formatInfoResolver = new FormatInfoResolver((FormatInfoResolverConfiguration)ConfigurationManager.GetSection("dynamicMediaFormatMappings"));
        }

        protected override IMediaCache MediaCache { get { return _mediaCache; } }
        protected override IStorageBackend StorageBackend { get { return _storageBackend; } }
        protected override IMediaTransformerFactory MediaTransformerFactory { get { return _mediaTransformerFactory; } }
        protected override IFormatInfoResolver FormatInfoResolver { get { return _formatInfoResolver; } }
    }
}