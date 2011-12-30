using System;
using System.IO;
using System.Windows.Media.Imaging;
using Bombsquad.Exif.Helpers;
using Bombsquad.Exif.Models;

namespace Bombsquad.Exif
{
	public class ExifParser : IExifParser
	{
		private const string DateTakenQuery = "/app1/ifd/{ushort=306}";
		private const string ISOSpeedQuery = "/app1/ifd/exif/subifd:{uint=34855}";
		private const string FNumberQuery = "/app1/ifd/exif/{ushort=33437}";
		private const string OrientationQuery = "/app1/ifd/{ushort=274}";
		private const string HorizontalResolutionQuery = "/app1/ifd/exif:{uint=282}";
		private const string VerticalResolutionQuery = "/app1/ifd/exif:{uint=283}";
		private const string ExposureTimeQuery = "/app1/ifd/exif/subifd:{uint=33434}";
		private const string ExposureCompensationQuery = "/app1/ifd/exif/subifd:{uint=37380}";
		private const string CreationSoftwareQuery = "/app1/ifd/exif:{uint=305}";
		private const string ColorRepresentationQuery = "/app1/ifd/exif/subifd:{uint=40961}";
		private const string LensApertureQuery = "/app1/ifd/exif/subifd:{uint=33437}";
		private const string FocalLengthQuery = "/app1/ifd/exif/subifd:{uint=37386}";
		private const string FlashModeQuery = "/app1/ifd/exif/subifd:{uint=37385}";
		private const string ExposureModeQuery = "/app1/ifd/exif/subifd:{uint=34850}";
		private const string WhiteBalanceQuery = "/app1/ifd/exif/subifd:{uint=37384}";

		private readonly ExifGpsInfoParser m_gpsInfoParser;

		public ExifParser()
		{
			m_gpsInfoParser = new ExifGpsInfoParser();
		}

        public ExifData Parse(Stream stream)
        {
            try
            {
                return Parse( BitmapFrame.Create(stream) );
            }
            catch
            {
                return null;
            }
        }

        public ExifData Parse(BitmapSource bitmap)
		{
			BitmapMetadata metadata;

			try
			{
				metadata = (BitmapMetadata) bitmap.Metadata;
			}
			catch
			{
				return null;
			}

			return new ExifData
			{
				DateTaken = GetDateTaken( metadata ),

				CameraManufacturer = GetStringValue( metadata, m => m.CameraManufacturer ),
				CameraModel = GetStringValue( metadata, m => m.CameraModel ),
				CreationSoftware = metadata.ReadString( CreationSoftwareQuery ),

				ImageWidth = bitmap.PixelWidth,
				ImageHeight = bitmap.PixelHeight,
				HorizontalResolution = metadata.ReadUnsignedRational( HorizontalResolutionQuery ),
				VerticalResolution = metadata.ReadUnsignedRational( VerticalResolutionQuery ),
				ImageOrientation = GetOrientation( metadata ),
				ColorRepresentation = GetColorRepresentation( metadata ),

				ISOSpeed = metadata.ReadUShort( ISOSpeedQuery ),
				FNumber = metadata.ReadUnsignedRational( FNumberQuery ),
				ExposureTime = metadata.ReadUnsignedRational( ExposureTimeQuery ),
				ExposureCompensation = metadata.ReadSignedRational( ExposureCompensationQuery ),
				LensAperture = metadata.ReadUnsignedRational( LensApertureQuery ),
				FocalLength = metadata.ReadUnsignedRational( FocalLengthQuery ),
				FlashMode = GetFlashMode( metadata ),
				ExposureMode = GetExposureMode( metadata ),
				WhiteBalanceMode = GetWhiteBalanceMode( metadata ),

				GpsData = m_gpsInfoParser.Parse( metadata )
			};
		}

		private static string GetStringValue( BitmapMetadata metadata, Func<BitmapMetadata,string> accessor )
		{
			try { return accessor( metadata ); } catch { return null; }
		}

		private static DateTime? GetDateTaken( BitmapMetadata metadata )
		{
			if( !string.IsNullOrEmpty( metadata.DateTaken ) )
			{
				DateTime result;
				if ( DateTime.TryParse( metadata.DateTaken, out result ) )
				{
					return result;
				}

				if(BitmapMetadataExtensions.TryParseExifDateString( metadata.DateTaken, out result ) )
				{
					return result;
				}
			}

			return metadata.ReadDateTime( DateTakenQuery );
		}

		private static ExifWhiteBalanceMode GetWhiteBalanceMode( BitmapMetadata metadata )
		{
			var mode = metadata.ReadUShort( WhiteBalanceQuery );
			return !mode.HasValue ? ExifWhiteBalanceMode.Unknown : (ExifWhiteBalanceMode) mode.Value;
		}

		private static ExifExposureMode GetExposureMode( BitmapMetadata metadata )
		{
			var mode = metadata.ReadUShort( ExposureModeQuery );
			return !mode.HasValue ? ExifExposureMode.Unknown : (ExifExposureMode) mode.Value;
		}

		private static ExifFlashMode GetFlashMode( BitmapMetadata metadata )
		{
			var value = metadata.ReadUShort( FlashModeQuery );
			return value.HasValue
				? (value.Value % 2 == 1 ? ExifFlashMode.FlashFired : ExifFlashMode.FlashDidNotFire)
				: ExifFlashMode.Unknown;
		}

		private static ExifColorRepresentation GetColorRepresentation( BitmapMetadata metadata )
		{
			var value = metadata.ReadUShort( ColorRepresentationQuery );
			return value.HasValue && value == 1 ? ExifColorRepresentation.sRGB : ExifColorRepresentation.Uncalibrated;
		}

		private static ExifOrientation GetOrientation( BitmapMetadata metadata )
		{
			var direction = metadata.ReadUShort( OrientationQuery );
			return direction.HasValue ? (ExifOrientation) direction.Value : ExifOrientation.None;
		}
	}
}
