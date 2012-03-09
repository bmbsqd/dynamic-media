using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations.Results
{
    internal class CopyToOutputStreamResult : IResult
    {
        public CopyToOutputStreamResult(DateTime lastModified, Stream stream)
        {
            LastModified = lastModified;
            Stream = stream;
        }

        public DateTime LastModified
        {
            get; private set;
        }

        public Stream Stream { get; private set; }

        public void Serve(HttpResponseBase response)
        {
            Stream.CopyTo(response.OutputStream);
        }

        public void Serve(HttpResponseBase response, long offset, long length)
        {
            var buffer = new byte[length];
            var bytesRead = Stream.Read(buffer, (int)offset, (int)length);
            response.OutputStream.Write(buffer, 0, bytesRead );
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}