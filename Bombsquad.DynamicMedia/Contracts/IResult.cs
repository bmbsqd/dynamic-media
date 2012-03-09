using System;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IResult : IDisposable
    {
        DateTime LastModified { get; }
        void Serve(HttpResponseBase response);
        void Serve(HttpResponseBase response, long offset, long length);
    }
}