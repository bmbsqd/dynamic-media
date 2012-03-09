using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Util;

namespace Bombsquad.DynamicMedia.Implementations.Results
{
	internal class CopyToOutputStreamResult : IResult
	{
		public CopyToOutputStreamResult( DateTime? lastModified, string etag, long contentLength, Stream stream )
		{
			LastModified = lastModified;
			ETag = etag;
			ContentLength = contentLength;
			Stream = stream;
		}

		public DateTime? LastModified { get; private set; }
		public string ETag { get; private set; }
		public long ContentLength { get; private set; }
		public Stream Stream { get; private set; }

		public void Serve( HttpResponseBase response )
		{
			Stream.CopyTo( response.OutputStream );
		}

		public void Serve( HttpResponseBase response, long offset, long length )
		{
			Stream.CopyTo( response.OutputStream, offset, length );
		}

		public void Dispose()
		{
			Stream.Dispose();
		}
	}
}