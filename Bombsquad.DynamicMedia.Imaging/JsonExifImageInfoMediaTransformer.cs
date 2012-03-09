using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Xml;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.FormatInfo;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class JsonExifImageInfoMediaTransformer : ExifImageInfoMediaTransformerBase
    {
        protected override bool TryGetOutputFormat(HttpRequestBase request, IFormatInfo originalFormat, out IFormatInfo outputFormat)
        {
            var format = request.QueryString["format"];
            if (string.Equals(format, "exif-json", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFormat = new FormatInfo { Extension = ".json", ContentType = "application/json" };
                return true;
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