using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations.Results
{
    internal class TransmitFileResult : IResult
    {
        public TransmitFileResult(DateTime? lastModified, string etag, long contentLength, string fileName)
        {
            LastModified = lastModified;
        	ETag = etag;
        	ContentLength = contentLength;
        	FileName = fileName;
        }

    	public DateTime? LastModified { get; private set; }
    	public string ETag { get; private set; }
		public long ContentLength { get; private set; }
        public string FileName { get; private set; }

        public void Serve(HttpResponseBase response)
        {
            response.TransmitFile(FileName);
        }

        public void Serve(HttpResponseBase response, long offset, long length)
        {
            response.TransmitFile(FileName, offset, length);
        }

        public void Dispose()
        {
        }
    }
}