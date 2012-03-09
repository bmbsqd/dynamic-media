using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class CompositeResultHandler : IResultHandler
    {
        private readonly IEnumerable<IResultHandler> _handlers;

        public CompositeResultHandler(params IResultHandler[] handlers )
        {
            _handlers = handlers;
        }

        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
            return _handlers.Any(handler => handler.HandleResult(result, outputFormat, request, response));
        }
    }
}