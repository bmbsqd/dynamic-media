using System.IO;
using System.Security.Cryptography;
using System.Text;
using Bombsquad.DynamicMedia.Contracts.ETag;
using System.Linq;

namespace Bombsquad.DynamicMedia.Implementations.ETag
{
    public class WeakFileInfoETagCalculator : IFileInfoETagCalculator
	{
		private readonly HashAlgorithm m_hashAlgorithm;

		public WeakFileInfoETagCalculator(HashAlgorithm hashAlgorithm)
		{
			m_hashAlgorithm = hashAlgorithm;
		}

		public WeakFileInfoETagCalculator()
		{
			m_hashAlgorithm = new SHA1Managed();
		}

		public string CalculateETag( FileInfo file )
		{
			var material = new StringBuilder();
			material.AppendLine( file.FullName );
			material.AppendLine( file.LastWriteTime.Ticks.ToString() );
			material.AppendLine( file.Length.ToString() );

			var hash = m_hashAlgorithm.ComputeHash( Encoding.UTF8.GetBytes( material.ToString() ) );
			return ETagUtil.CreateWeakETag( new string( hash.SelectMany( b => b.ToString("x2").ToLower() ).ToArray() ) );
		}
	}
}