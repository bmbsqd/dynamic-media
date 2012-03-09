using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.FormatInfo;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class XmlExifImageInfoMediaTransformer : ExifImageInfoMediaTransformerBase
    {
        protected override bool TryGetOutputFormat(HttpRequestBase request, IFormatInfo originalFormat, out IFormatInfo outputFormat)
        {
            var format = request.QueryString["format"];
            if (string.Equals(format, "exif-xml", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFormat = new FormatInfo { Extension = ".xml", ContentType = "text/xml" };
                return true;
            }

            outputFormat = null;
            return false;
        }

        protected override void Serialize(Stream output, ExifData exifData)
        {
            var serializer = new DataContractSerializer(typeof(ExifData));
            var writer = XmlWriter.Create(output);
            serializer.WriteObject(writer, exifData);
            writer.Flush();
        }
    }
}