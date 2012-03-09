using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bombsquad.DynamicMedia.Contracts.ETag;
using System.Linq;

namespace Bombsquad.DynamicMedia.Implementations.ETag
{
	public class WeakFileInfoETagCalculator : IFileInfoETagCalculator
	{
		private readonly HashAlgorithm m_hash;

		public WeakFileInfoETagCalculator(HashAlgorithm hash)
		{
			m_hash = hash;
		}

		public WeakFileInfoETagCalculator()
		{
			m_hash = new SHA1Managed();
		}

		public string CalculateETag( FileInfo file )
		{
			var material = new StringBuilder();
			material.AppendLine( file.FullName );
			material.AppendLine( file.LastWriteTime.Ticks.ToString() );
			material.AppendLine( file.Length.ToString() );

			var hash = m_hash.ComputeHash( Encoding.UTF8.GetBytes( material.ToString() ) );
			return new string( hash.SelectMany( b => b.ToString("x2").ToLower() ).ToArray() );
		}
	}
}