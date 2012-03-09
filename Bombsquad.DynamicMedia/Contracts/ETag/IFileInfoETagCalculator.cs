using System.IO;

namespace Bombsquad.DynamicMedia.Contracts.ETag
{
	public interface IFileInfoETagCalculator
	{
		string CalculateETag( FileInfo file );
	}
}