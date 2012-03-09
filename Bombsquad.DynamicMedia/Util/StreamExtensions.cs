using System;
using System.IO;

namespace Bombsquad.DynamicMedia.Util
{
	internal static class StreamExtensions
	{
		public static void CopyTo(this Stream source, Stream target, long offset, long length, int bufferSize = 4096)
		{
			source.Seek( offset, SeekOrigin.Begin );

			var buffer = new byte[bufferSize];
			long totalBytesRead = 0;

			while ( totalBytesRead < length )
			{
				var bytesToRead = (int)Math.Min( bufferSize, length - totalBytesRead );
				totalBytesRead += source.Read( buffer, 0, bytesToRead );
				target.Write( buffer, 0, bytesToRead );
			}

			target.Flush();
		}
	}
}