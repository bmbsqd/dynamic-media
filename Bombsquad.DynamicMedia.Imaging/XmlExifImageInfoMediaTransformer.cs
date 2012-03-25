using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class XmlExifImageInfoMediaTransformer : ExifImageInfoMediaTransformerBase
    {
        protected override bool TryGetOutputFormat(HttpRequestBase request, IFormatInfo originalFormat, IFormatInfoProvider formatInfoProvider, out IFormatInfo outputFormat)
        {
            var format = request.QueryString["format"];
            if (string.Equals(format, "exif-xml", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFormat = formatInfoProvider.ResolveFromExtension(".xml");
                return outputFormat != null;
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