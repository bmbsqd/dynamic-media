using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
    public class NullMediaCache : IMediaCache
    {
        public bool TryServeRequestFromCache(HttpRequestBase request, IFormatInfo outputFormat, out IResult result)
        {
            result = null;
            return false;
        }

        public bool TryAddToCache(HttpRequestBase request, Stream stream, IFormatInfo outputFormat, out IAddToCacheResult result)
        {
            result = null;
            return false;
        }

        public bool CacheOriginals
        {
            get { return false; }
        }
    }
}