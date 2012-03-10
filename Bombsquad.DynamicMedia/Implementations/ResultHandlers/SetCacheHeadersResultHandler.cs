using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class SetCacheHeadersResultHandler : IResultHandler
    {
        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
            if (result.LastModified.HasValue)
            {
                response.Cache.SetLastModified(result.LastModified.Value);
            }

            if (!string.IsNullOrEmpty(result.ETag))
            {
                response.Cache.SetETag("\"" + result.ETag + "\"");
            }

            return false;
        }
    }
}