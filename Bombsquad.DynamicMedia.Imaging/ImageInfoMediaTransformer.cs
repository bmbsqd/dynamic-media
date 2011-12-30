using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Windows.Media.Imaging;
using System.Xml;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.Exif;
using Bombsquad.Exif.Models;

namespace Bombsquad.DynamicMedia.Imaging
{
    public class ImageInfoMediaTransformer : IMediaTransformer
    {
        private readonly IExifParser _exifParser;

        public ImageInfoMediaTransformer(IExifParser exifParser, IFormatInfo outputFormat)
        {
            _exifParser = exifParser;
            OutputFormat = outputFormat;
        }

        public Stream TransformStream(HttpRequestBase request, Stream stream)
        {
            var bitmap = BitmapFrame.Create(stream);
            var exifData = _exifParser.Parse(bitmap);
            var output = new MemoryStream();

            if (OutputFormat.ContentType == "application/json")
            {
                RenderJson(exifData, output);
            }
            
            if( OutputFormat.ContentType == "text/xml" )
            {
                RenderXml(exifData, output);
            }

            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        private void RenderXml(ExifData exifData, MemoryStream output)
        {
            var serializer = new DataContractSerializer(typeof(ExifData));
            var writer = XmlWriter.Create(output);
            serializer.WriteObject(writer, exifData);
            writer.Flush();
        }

        private void RenderJson(ExifData exifData, MemoryStream output)
        {
            var serializer = new DataContractJsonSerializer(typeof(ExifData));
            serializer.WriteObject(output, exifData);
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath;
        }
    }
}