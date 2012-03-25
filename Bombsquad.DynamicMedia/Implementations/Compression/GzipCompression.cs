using System.IO;
using System.IO.Compression;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Compression
{
    public class GzipCompression : ICompression
    {
        public string ContentEncoding
        {
            get { return "gzip"; }
        }

        public Stream Compress(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }
    }
}