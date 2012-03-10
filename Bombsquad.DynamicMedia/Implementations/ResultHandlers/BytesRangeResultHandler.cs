using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
	public class BytesRangeResultHandler : IResultHandler
	{
		private const string RangeByteHeaderStart = "bytes=";

		public bool HandleResult( IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response )
		{
			Range range;
			if ( !TryGetRequestedRange( request, out range ) )
			{
				return false;
			}

            if (!ValidateIfRangeHeader(request, result))
            {
                return false;
            }

			var offset = range.Start ?? 0;
			var end = range.End.HasValue ? range.End.Value : result.ContentLength - 1;
			var length = end - offset + 1;

			response.AddHeader( "Accept-Ranges", "bytes" );
			response.AddHeader( "Content-Range", "bytes " + offset + "-" + end + "/" + result.ContentLength );
			response.StatusCode = 206;

			result.Serve( response, offset, length );
			return true;
		}

	    private bool TryGetRequestedRange( HttpRequestBase request, out Range range )
		{
			var rangeHeader = request.Headers[ "Range" ];
			if ( string.IsNullOrEmpty( rangeHeader ) )
			{
				range = null;
				return false;
			}

			if ( !rangeHeader.StartsWith( RangeByteHeaderStart ) )
			{
				range = null;
				return false;
			}

			var parts = rangeHeader.Substring( RangeByteHeaderStart.Length ).Split( '-' );

			if ( parts.Length != 2 )
			{
				range = null;
				return false;
			}

			range = new Range
			{
				Start = string.IsNullOrEmpty( parts[ 0 ] ) ? (long?) null : long.Parse( parts[ 0 ] ),
				End = string.IsNullOrEmpty( parts[ 1 ] ) ? (long?) null : long.Parse( parts[ 1 ] )
			};
			return true;
		}

	    private bool ValidateIfRangeHeader(HttpRequestBase request, IResult result)
	    {
	        var ifRangeHeader = request.Headers["If-Range"];
	        if(string.IsNullOrEmpty(ifRangeHeader))
	        {
	            return true;
	        }

	        return string.Equals(ifRangeHeader, result.ETag);
	    }

	    private class Range
		{
			public long? Start { get; set; }
			public long? End { get; set; }
		}
	}
}