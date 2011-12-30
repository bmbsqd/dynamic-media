using System.IO;
using System.Windows.Media.Imaging;
using Bombsquad.Exif.Models;

namespace Bombsquad.Exif
{
	public interface IExifParser
	{
	    ExifData Parse(BitmapSource bitmap);
		ExifData Parse( Stream stream );
	}
}