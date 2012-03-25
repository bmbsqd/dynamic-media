using System.Configuration;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia;
using Bombsquad.DynamicMedia.CombineCss;
using Bombsquad.DynamicMedia.Configuration;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Storage;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Imaging;
using Bombsquad.DynamicMedia.Implementations.Cache;
using Bombsquad.DynamicMedia.Implementations.ETag;
using Bombsquad.DynamicMedia.Implementations.FormatInfo;
using Bombsquad.DynamicMedia.Implementations.Storage;
using Bombsquad.DynamicMedia.Implementations.Transformation;
using Bombsquad.DynamicMedia.Less;
using Bombsquad.DynamicMedia.Markdown;
using Bombsquad.DynamicMedia.Minify;

namespace DemoSite
{
	public class DynamicMediaHandler : DynamicMediaHandlerBase
	{
		private readonly IMediaCache m_mediaCache;
		private readonly IStorageBackend m_storageBackend;
		private readonly IMediaTransformerFactory m_mediaTransformerFactory;
		private readonly FormatInfoProvider _mFormatInfoProvider;

		public DynamicMediaHandler()
		{
			var storageRoot = new DirectoryInfo( HttpContext.Current.Server.MapPath( ConfigurationManager.AppSettings[ "StorageRoot" ] ) );
            var cacheRoot = new DirectoryInfo( HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["CacheRoot"]));
            
            var etagCalculator = new WeakFileInfoETagCalculator();

			m_mediaCache = new FileSystemMediaCache(etagCalculator, cacheRoot);
            //m_mediaCache = new NullMediaCache();
		    m_storageBackend = new FileSystemStorageBackend( storageRoot, etagCalculator );
			m_mediaTransformerFactory = new CompositeMediaTransformerFactory(
				new XmlExifImageInfoMediaTransformer(), 
                new JsonExifImageInfoMediaTransformer(),
				new ImageMediaTransformerFactory(), 
                new CssLessMediaTransformerFactory(), 
                new CombineCssMediaTransformerFactory(),
				new EmbedAsBase64CssMediaTransformerFactory(), 
                new CssMinifyingMediaTransformerFactory(),
				new JavascriptMinifyingMediaTransformerFactory(), 
                new MarkdownMediaTransformerFactory()
			);

			_mFormatInfoProvider = new FormatInfoProvider( (FormatInfoResolverConfiguration) ConfigurationManager.GetSection( "dynamicMediaFormatMappings" ) );
		}

		protected override bool CacheOriginals
		{
			get { return false; }
		}

		protected override IMediaCache MediaCache
		{
			get { return m_mediaCache; }
		}

		protected override IStorageBackend StorageBackend
		{
			get { return m_storageBackend; }
		}

		protected override IMediaTransformerFactory MediaTransformerFactory
		{
			get { return m_mediaTransformerFactory; }
		}

		protected override IFormatInfoProvider FormatInfoProvider
		{
			get { return _mFormatInfoProvider; }
		}
	}
}