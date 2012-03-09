using System.IO;
using Bombsquad.DynamicMedia.Contracts.ETag;

namespace Bombsquad.DynamicMedia.Implementations.ETag
{
	public class NullFileInfoETagCalculator : IFileInfoETagCalculator
	{
		public string CalculateETag( FileInfo file )
		{
			return null;
		}
	}
}