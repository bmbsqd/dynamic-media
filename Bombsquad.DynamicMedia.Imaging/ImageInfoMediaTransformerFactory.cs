using System;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.Exif;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class ImageInfoMediaTransformerFactory : IMediaTransformerFactory
    {
        private readonly ExifParser _exifParser;

        public ImageInfoMediaTransformerFactory()
        {
            _exifParser = new ExifParser();
        }

        public bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer)
        {
            if(!string.Equals( originalFormat.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase ) )
            {
                mediaTransformer = null;
                return false;
            }

            FormatInfo formatInfo;
            if(!TryGetOutputFormat(request.QueryString["format"], out formatInfo))
            {
                mediaTransformer = null;
                return false;
            }

            mediaTransformer = new ImageInfoMediaTransformer(_exifParser, formatInfo);
            return true;
        }

        private bool TryGetOutputFormat(string format, out FormatInfo formatInfo)
        {
            if(string.IsNullOrEmpty(format))
            {
                formatInfo = null;
                return false;
            }

            switch (format.ToLower())
            {
                case "exif-xml":
                    formatInfo = new FormatInfo {Extension = ".xml", ContentType = "text/xml"};
                    return true;

                case "exif-json":
                    formatInfo = new FormatInfo { Extension = ".json", ContentType = "application/json" };
                    return true;

                default:
                    formatInfo = null;
                    return false;
            }
        }
    }
}