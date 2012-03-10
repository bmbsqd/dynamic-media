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
            var ifNoneMatchHeader = request.Headers["If-None-Match"];
            var ifModifiedSinceHeader = request.Headers["If-Modified-Since"];

            if (string.IsNullOrEmpty(ifNoneMatchHeader) && string.IsNullOrEmpty(ifModifiedSinceHeader))
            {
                return false;
            }

            if (!ValidateETag(ifNoneMatchHeader, result))
            {
                return false;
            }

            if (!ValidateLastModifyDate(ifModifiedSinceHeader, result))
            {
                return false;
            }

            response.StatusCode = 304;
            response.SuppressContent = true;
            return true;
        }

        private bool ValidateETag(string ifNoneMatchHeader, IResult result)
        {
            if (string.IsNullOrEmpty(ifNoneMatchHeader))
            {
                return true;
            }

            return string.Equals(ifNoneMatchHeader, result.ETag);
        }

        private static bool ValidateLastModifyDate(string ifModifiedSinceHeader, IResult result)
        {
            if (string.IsNullOrEmpty(ifModifiedSinceHeader))
            {
                return true;
            }

            if (!result.LastModified.HasValue)
            {
                return false;
            }

            DateTime modifiedSince;
            if (string.IsNullOrEmpty(ifModifiedSinceHeader) || !DateTime.TryParse(ifModifiedSinceHeader, out modifiedSince))
            {
                return false;
            }

            if (modifiedSince <= result.LastModified.Value.Subtract(TimeSpan.FromSeconds(1)))
            {
                return false;
            }

            return true;
        }
    }
}