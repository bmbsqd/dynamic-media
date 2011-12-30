using System.Drawing;
using System.Runtime.Serialization;

namespace Bombsquad.Exif.Models
{
	public static class ExifOrientationExtensions
	{
		public static RotateFlipType GetRotateFlipType( this ExifOrientation orientation )
		{
			switch ( orientation )
			{
				case ExifOrientation.HorizontalFlip:
					return RotateFlipType.RotateNoneFlipX;

				case ExifOrientation.Rotate180:
					return RotateFlipType.Rotate180FlipNone;

				case ExifOrientation.VerticalFlip:
					return RotateFlipType.Rotate180FlipX;

				case ExifOrientation.Transpose:
					return RotateFlipType.Rotate90FlipX;

				case ExifOrientation.Rotate270:
					return RotateFlipType.Rotate90FlipNone;

				case ExifOrientation.Transverse:
					return RotateFlipType.Rotate270FlipX;

				case ExifOrientation.Rotate90:
					return RotateFlipType.Rotate270FlipNone;

				default:
					return RotateFlipType.RotateNoneFlipNone;
			}
		}
	}

	public enum ExifOrientation
	{
		None = 0,  
		Normal = 1,  
		HorizontalFlip = 2,  
		Rotate180 = 3,  
		VerticalFlip = 4,  
		Transpose = 5,  
		Rotate270 = 6,  
		Transverse = 7,  
		Rotate90 = 8
	}
}