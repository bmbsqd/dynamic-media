using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Contracts.Cache
{
    public interface IMediaCache
    {
        bool TryServeRequestFromCache(HttpRequestBase request, IFormatInfo outputFormat, out IResult result);
        bool TryAddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat, out IAddToCacheResult result);
    }
}