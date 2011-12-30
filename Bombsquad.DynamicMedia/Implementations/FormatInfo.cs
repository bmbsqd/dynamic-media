using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
{
    public class FormatInfo : IFormatInfo
    {
        public string Extension { get; set; }
        public string ContentType { get; set; }
    }
}