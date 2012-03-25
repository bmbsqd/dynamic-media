using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class JsonExifImageInfoMediaTransformer : ExifImageInfoMediaTransformerBase
    {
        protected override bool TryGetOutputFormat(HttpRequestBase request, IFormatInfo originalFormat, IFormatInfoProvider formatInfoProvider, out IFormatInfo outputFormat)
        {
            var format = request.QueryString["format"];
            if (string.Equals(format, "exif-json", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFormat = formatInfoProvider.ResolveFromExtension(".json");
                return outputFormat != null;
            }

            outputFormat = null;
            return false;
        }

        protected override void Serialize(Stream output, ExifData exifData)
        {
            if (output == null) throw new ArgumentNullException("output");
            var serializer = new DataContractJsonSerializer(typeof(ExifData));
            serializer.WriteObject(output, exifData);
        }
    }
}