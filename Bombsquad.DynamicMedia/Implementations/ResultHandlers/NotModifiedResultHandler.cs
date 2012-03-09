using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class NotModifiedResultHandler : IResultHandler
    {
        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
            var ifModifiedSince = request.Headers["if-modified-since"];
            DateTime modifiedSince;
            if (string.IsNullOrEmpty(ifModifiedSince) || !DateTime.TryParse(ifModifiedSince, out modifiedSince))
            {
                return false;
            }

            if (modifiedSince > result.LastModified.Subtract(TimeSpan.FromSeconds(1)))
            {
                response.StatusCode = 304;
                response.SuppressContent = true;
                return true;
            }

            return false;
        }
    }
}