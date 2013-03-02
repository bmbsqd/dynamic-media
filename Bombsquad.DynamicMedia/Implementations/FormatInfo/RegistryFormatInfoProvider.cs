using System;
using System.Collections.Concurrent;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Microsoft.Win32;

namespace Bombsquad.DynamicMedia.Implementations.FormatInfo
{
	public class RegistryFormatInfoProvider : IFormatInfoProvider
	{
		private readonly TimeSpan? m_clientCacheMaxAge;
		private readonly ConcurrentDictionary<string, IFormatInfo> m_formatInfoCache;

		public RegistryFormatInfoProvider( TimeSpan? clientCacheMaxAge )
		{
			m_clientCacheMaxAge = clientCacheMaxAge;
			m_formatInfoCache = new ConcurrentDictionary<string, IFormatInfo>();
		}

		public IFormatInfo ResolveFromExtension( string extension )
		{
			if ( string.IsNullOrEmpty( extension ) || !extension.StartsWith( "." ) )
			{
				return null;
			}

			return m_formatInfoCache.GetOrAdd( extension, key =>
			{
				var contentType = Registry.GetValue( @"HKEY_CLASSES_ROOT\" + extension, "Content Type", null ) as string;

				if ( string.IsNullOrEmpty( contentType ) )
				{
					return null;
				}

				return new FormatInfo
				{
					Extension = extension,
					ContentType = contentType,
					AllowCompression = false,
					ClientCacheMaxAge = m_clientCacheMaxAge
				};
			} );
		}

		private class FormatInfo : IFormatInfo
		{
			public string Extension { get; set; }
			public string ContentType { get; set; }
			public bool AllowCompression { get; set; }
			public TimeSpan? ClientCacheMaxAge { get; set; }
		}
	}
}