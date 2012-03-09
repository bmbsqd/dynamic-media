using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.FormatInfo
{
    public class FormatInfo : IFormatInfo
    {
        public string Extension { get; set; }
        public string ContentType { get; set; }
    }
}