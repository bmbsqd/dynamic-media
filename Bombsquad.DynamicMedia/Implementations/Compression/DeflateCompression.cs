using System.IO;
using System.IO.Compression;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Compression
{
    public class DeflateCompression : ICompression
    {
        public string ContentEncoding
        {
            get { return "deflate"; }
        }

        public Stream Compress(Stream stream)
        {
            return new DeflateStream( stream, CompressionMode.Compress );
        }
    }
}