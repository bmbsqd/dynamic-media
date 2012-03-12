using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Contracts.Cache
{
    public interface IMediaCache
    {
        bool TryServeRequestFromCache(HttpRequestBase request, IFormatInfo outputFormat, out IResult result);
        bool TryAddToCache(HttpRequestBase request, Func<Stream> stream, IFormatInfo outputFormat, out IAddToCacheResult result);
    }
}