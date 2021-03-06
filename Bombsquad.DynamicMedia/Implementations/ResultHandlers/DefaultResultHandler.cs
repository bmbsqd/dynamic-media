using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class DefaultResultHandler : IResultHandler
    {
        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
        	result.Serve(response);
            return true;
        }
    }
}