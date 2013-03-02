using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Implementations.Results
{
	public class HeadResultWrapper : IResult
	{
		private readonly IResult m_result;

		public HeadResultWrapper( IResult result )
		{
			m_result = result;
		}

		public void Serve( HttpResponseBase response )
		{
			response.AddHeader( "Content-Length", Convert.ToString( ContentLength ) );
		}

		public void Serve( HttpResponseBase response, long offset, long length )
		{
			response.AddHeader( "Content-Length", Convert.ToString( length ) );
		}

		public long ContentLength
		{
			get
			{
				return m_result.ContentLength;
			}
		}

		public DateTime? LastModified
		{
			get
			{
				return m_result.LastModified;
			}
		}

		public string ETag
		{
			get
			{
				return m_result.ETag;
			}
		}

		public void Dispose()
		{
			m_result.Dispose();
		}
	}
}