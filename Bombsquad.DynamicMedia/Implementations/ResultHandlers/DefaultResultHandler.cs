using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class DefaultResultHandler : IResultHandler
    {
        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
            SetupDefaultHeaders( result, outputFormat, response );
        	result.Serve(response);
            return true;
        }

		public static void SetupDefaultHeaders( IResult result, IFormatInfo outputFormat, HttpResponseBase response )
		{
			response.ContentType = outputFormat.ContentType;
			NotModifiedResultHandler.SetCacheHeaders( result, response );
		}
    }
}