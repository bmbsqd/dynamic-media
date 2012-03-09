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
using Bombsquad.DynamicMedia.Implementations.ETag;
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
		private readonly IMediaCache m_mediaCache;
		private readonly IStorageBackend m_storageBackend;
		private readonly IMediaTransformerFactory m_mediaTransformerFactory;
		private readonly FormatInfoResolver m_formatInfoResolver;
		private readonly IResultHandler m_resultHandler;

		public DynamicMediaHandler()
		{
			var storageRoot =
				new DirectoryInfo( HttpContext.Current.Server.MapPath( ConfigurationManager.AppSettings[ "StorageRoot" ] ) );

			m_mediaCache = new NullMediaCache();
			m_storageBackend = new FileSystemStorageBackend( storageRoot, new WeakFileInfoETagCalculator() );
			m_mediaTransformerFactory = new CompositeMediaTransformerFactory( new IMediaTransformerFactory[]
			{
				new XmlExifImageInfoMediaTransformer(), new JsonExifImageInfoMediaTransformer(),
				new ImageMediaTransformerFactory(), new CssLessMediaTransformerFactory(), new CombineCssMediaTransformerFactory(),
				new EmbedAsBase64CssMediaTransformerFactory(), new CssMinifyingMediaTransformerFactory(),
				new JavascriptMinifyingMediaTransformerFactory(), new MarkdownMediaTransformerFactory()
			} );

			m_formatInfoResolver = new FormatInfoResolver( (FormatInfoResolverConfiguration) ConfigurationManager.GetSection( "dynamicMediaFormatMappings" ) );

			m_resultHandler = new CompositeResultHandler( new NotModifiedResultHandler(), new BytesRangeResultHandler(), new DefaultResultHandler() );
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

		protected override IFormatInfoResolver FormatInfoResolver
		{
			get { return m_formatInfoResolver; }
		}

		protected override IResultHandler ResultHandler
		{
			get { return m_resultHandler; }
		}
	}
}