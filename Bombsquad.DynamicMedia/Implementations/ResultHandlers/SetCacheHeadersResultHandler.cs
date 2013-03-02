using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class SetCacheHeadersResultHandler : IResultHandler
    {
        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
			response.Cache.SetCacheability( HttpCacheability.Public );

			if ( outputFormat.ClientCacheMaxAge.HasValue )
			{
				response.Cache.SetNoServerCaching();
				response.Cache.SetExpires( DateTime.Now.Add( outputFormat.ClientCacheMaxAge.Value ) );
				response.Cache.SetMaxAge( outputFormat.ClientCacheMaxAge.Value );
			}

            if (result.LastModified.HasValue)
            {
                response.Cache.SetLastModified(result.LastModified.Value);
            }

            if (!string.IsNullOrEmpty(result.ETag))
            {
				response.Cache.SetETag( result.ETag );
            }

            return false;
        }
    }
}