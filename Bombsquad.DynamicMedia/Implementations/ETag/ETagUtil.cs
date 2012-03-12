namespace Bombsquad.DynamicMedia.Implementations.ETag
{
	public class ETagUtil
	{
		public static string CreateStrongETag(string value)
		{
			return "\"" + value + "\"";
		}

		public static string CreateWeakETag(string value)
		{
			return "W/\"" + value + "\"";
		}
	}
}
