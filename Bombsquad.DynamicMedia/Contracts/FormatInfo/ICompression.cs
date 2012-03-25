using System.IO;

namespace Bombsquad.DynamicMedia.Contracts.FormatInfo
{
    public interface ICompression
    {
        string ContentEncoding { get; }
        Stream Compress(Stream stream);
    }
}