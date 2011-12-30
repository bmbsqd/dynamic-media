using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations
{
    public class NullMediaCache : IMediaCache
    {
        public bool TryServeRequestFromCache(HttpRequestBase request, HttpResponseBase response, IFormatInfo outputFormat)
        {
            return false;
        }

        public void AddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat)
        {
        }
    }
}