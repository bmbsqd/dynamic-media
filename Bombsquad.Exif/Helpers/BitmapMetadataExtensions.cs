using System;
using System.Windows.Media.Imaging;

namespace Bombsquad.Exif.Helpers
{
	internal static class BitmapMetadataExtensions
	{
		private static object ReadObject( this BitmapMetadata metadata, string query )
		{
			return metadata.ContainsQuery( query ) ? metadata.GetQuery( query ) : null;
		}

		public static byte? ReadByte( this BitmapMetadata metadata, string query )
		{
			return (byte?) metadata.ReadObject( query );
		}

		public static ushort? ReadUShort( this BitmapMetadata metadata, string query )
		{
			return (ushort?) metadata.ReadObject( query );
		}

		public static long? ReadLong( this BitmapMetadata metadata, string query )
		{
			return (long?) metadata.ReadObject( query );
		}

		public static ulong? ReadULong( this BitmapMetadata metadata, string query )
		{
			return (ulong?) metadata.ReadObject( query );
		}

		public static ulong[] ReadULongArray( this BitmapMetadata metadata, string query )
		{
			return metadata.ReadObject( query ) as ulong[];
		}

		public static ushort [] ReadUShortArray( this BitmapMetadata metadata, string query )
		{
			return metadata.ReadObject( query ) as ushort[];
		}

		public static string ReadString( this BitmapMetadata metadata, string query )
		{
			return (string) metadata.ReadObject( query );
		}

		public static double? ReadSignedRational( this BitmapMetadata metadata, string query )
		{
			var value = metadata.ReadLong( query );
			return value.HasValue
				? ((value.Value & 0xFFFFFFFFL) / (double) ((value.Value & 0x7FFFFFFF00000000L) >> 32))
				: (double?) null;
		}

		public static double? ReadUnsignedRational( this BitmapMetadata metadata, string query )
		{
			var value = metadata.ReadULong( query );
			return value.HasValue
				? ConvertToUnsignedRational( value.Value )
				: (double?) null;
		}

		public static double ConvertToUnsignedRational( ulong value )
		{
			return (value & 0xFFFFFFFFL) / (double) ((value & 0xFFFFFFFF00000000L) >> 32);
		}

		public static DateTime? ReadDateTime( this BitmapMetadata metadata, string query )
		{
			DateTime d;
			return TryParseExifDateString( metadata.ReadString( query ), out d ) ? (DateTime?) d : null;
		}

		public static bool TryParseExifDateString( string format, out DateTime dateTime )
		{
			if ( string.IsNullOrEmpty( format ) || format.StartsWith( "0000" ) )
			{
				dateTime = DateTime.MinValue;
				return false;
			}

			var tokens = format.Split( ':', ' ' );
			var year = int.Parse( tokens[ 0 ] );
			var month = int.Parse( tokens[ 1 ] );
			var day = int.Parse( tokens[ 2 ] );
			var hour = int.Parse( tokens[ 3 ] );
			var minute = int.Parse( tokens[ 4 ] );
			var second = int.Parse( tokens[ 5 ] );

			dateTime = new DateTime( year, month, day, hour, minute, second );
			return true;
		}
	}
}