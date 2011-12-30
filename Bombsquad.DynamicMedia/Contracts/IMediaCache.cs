using System.IO;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IMediaCache
    {
        bool TryServeRequestFromCache(HttpRequestBase request, HttpResponseBase response, IFormatInfo outputFormat);
        void AddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat);
    }
}