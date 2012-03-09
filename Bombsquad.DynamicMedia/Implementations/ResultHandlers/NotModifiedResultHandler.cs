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
			if ( TryHandleIfNoneMatchHeader( result, request, response ) )
			{
				return true;
			}

        	if(TryHandleIfModifiedSinceHeader( result, request, response ))
        	{
        		return true;
        	}

        	return false;
        }

		private bool TryHandleIfNoneMatchHeader( IResult result, HttpRequestBase request, HttpResponseBase response )
		{
			if(string.IsNullOrEmpty(result.ETag))
			{
				return false;
			}

			var ifNoneMatch = request.Headers[ "If-None-Match" ];
			if(string.IsNullOrEmpty( ifNoneMatch ))
			{
				return false;
			}

			if ( !string.Equals( result.ETag, ifNoneMatch ) )
			{
				return false;
			}

			ServeNotModifiedResult( result, response );
			return true;
		}

		private static bool TryHandleIfModifiedSinceHeader( IResult result, HttpRequestBase request, HttpResponseBase response )
		{
			if(!result.LastModified.HasValue)
			{
				return false;
			}

			var ifModifiedSince = request.Headers[ "If-Modified-Since" ];
			DateTime modifiedSince;
			if ( string.IsNullOrEmpty( ifModifiedSince ) || !DateTime.TryParse( ifModifiedSince, out modifiedSince ) )
			{
				return false;
			}

			if ( modifiedSince <= result.LastModified.Value.Subtract( TimeSpan.FromSeconds( 1 ) ) )
			{
				return false;
			}

			ServeNotModifiedResult( result, response );
			return true;
		}


		private static void ServeNotModifiedResult( IResult result, HttpResponseBase response )
		{
			SetCacheHeaders( result, response );

			response.StatusCode = 304;
			response.SuppressContent = true;
		}

		public static void SetCacheHeaders( IResult result, HttpResponseBase response )
		{
			if ( result.LastModified.HasValue )
			{
				response.Cache.SetLastModified( result.LastModified.Value );
			}

			if ( !string.IsNullOrEmpty( result.ETag ) )
			{
				response.Cache.SetETag( "\"" + result.ETag + "\"" );
			}
		}
    }
}