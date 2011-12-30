using System;
using System.Linq;
using System.Windows.Media.Imaging;
using Bombsquad.Exif.Helpers;
using Bombsquad.Exif.Models;

namespace Bombsquad.Exif
{
	internal class ExifGpsInfoParser
	{
		private const string GpsVersionQuery = "/app1/ifd/Gps/subifd:{uint=0}";
		private const string ProcessingMethodQuery = "/app1/ifd/Gps/subifd:{uint=27}";
		private const string MeasureModeQuery = "/app1/ifd/Gps/subifd:{uint=10}"; 
		private const string LatitudeQuery = "/app1/ifd/gps/subifd:{ulong=2}";
		private const string LongitudeQuery = "/app1/ifd/gps/subifd:{ulong=4}";
		private const string NorthOrSouthQuery = "/app1/ifd/gps/subifd:{char=1}";
		private const string EastOrWestQuery = "/app1/ifd/gps/subifd:{char=3}";
		private const string AltitudeQuery = "/app1/ifd/Gps/subifd:{uint=6}";
		private const string AltitudeReferenceQuery = "/app1/ifd/Gps/subifd:{uint=5}";

		internal ExifGpsData Parse( BitmapMetadata metadata )
		{
			return new ExifGpsData
			{
				GpsVersion = ParseGpsVersion( metadata ),
				ProcessingMethod = metadata.ReadString( ProcessingMethodQuery ),
				MeasureMode = ParseMeasureMode( metadata ),
				Latitude = ParseLatitude( metadata ),
				Longitude = ParseLongitude( metadata ),
				Altitude = ParseAltitude( metadata ) 
			};
		}

		private static ExifGpsMeasureMode ParseMeasureMode( BitmapMetadata metadata )
		{
			var measureMode = metadata.ReadByte( MeasureModeQuery );
			return measureMode.HasValue ? (ExifGpsMeasureMode) measureMode.Value : ExifGpsMeasureMode.Unknown;
		}

		private static double? ParseAltitude( BitmapMetadata metadata )
		{
			var altitude = metadata.ReadUnsignedRational( AltitudeQuery );
			if( ! altitude.HasValue )
			{
				return null;
			}

			var altitudeReference = metadata.ReadByte( AltitudeReferenceQuery );
			if( altitudeReference.HasValue && altitudeReference.Value == 1 )
			{
				altitude = -altitude;
			}
			return altitude;
		}

		private static decimal? ParseGpsVersion( BitmapMetadata metadata )
		{
			var version = metadata.ReadUShortArray( GpsVersionQuery );
			if( version == null )
			{
				return null;
			}

			return version.Select( ( t, i ) => (decimal) Math.Pow( 10, -i ) * t ).Sum();
		}

		private static double? ParseLatitude( BitmapMetadata metadata )
		{
			return ParseCoordinate( metadata, LatitudeQuery, NorthOrSouthQuery, "S" );
		}

		private static double ? ParseLongitude( BitmapMetadata metadata )
		{
			return ParseCoordinate( metadata, LongitudeQuery, EastOrWestQuery, "W" );
		}

		private static double? ParseCoordinate( BitmapMetadata metadata, string coordinateQuery, string directionQuery, string invertDirection )
		{
			var coordinates = metadata.ReadULongArray( coordinateQuery );
			if ( coordinates != null )
			{
				var coordinate = ConvertCoordinate( coordinates );

				var direction = metadata.ReadString( directionQuery );
				if ( direction == invertDirection )
				{
					coordinate = -coordinate;
				}

				return coordinate;
			}

			return null;
		}

		private static double ConvertCoordinate( ulong[] coordinates )
		{
			var degrees = BitmapMetadataExtensions.ConvertToUnsignedRational( coordinates[ 0 ] );
			var minutes = BitmapMetadataExtensions.ConvertToUnsignedRational( coordinates[ 1 ] );
			var seconds = BitmapMetadataExtensions.ConvertToUnsignedRational( coordinates[ 2 ] );
			return degrees + (minutes / 60.0) + (seconds / 3600);
		}
	}
}