using System;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IResult : IDisposable
    {
    	long ContentLength { get; }
        DateTime? LastModified { get; }
    	string ETag { get; }

    	void Serve(HttpResponseBase response);
        void Serve(HttpResponseBase response, long offset, long length);
    }
}